import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import router from '@/router';
import { DateTime } from 'luxon';

import { DataService, ScoreService, MappingService, ChartService } from '@/services/';
import { InfrastructureComponentSummary, ScoreHistoryItem, SeverityFilter } from '@/models';

@Component
export default class ComponentDetail extends Vue {

    @Prop()
    id!: string;

    @Prop({ default: null })
    date!: string;

    severityFilter: SeverityFilter = new SeverityFilter();
    selectedDate?: DateTime;
    selectedScore: number = 0;
    loaded: boolean = false;
    loadFailed: boolean = false;
    service: DataService = new DataService();
    data: InfrastructureComponentSummary = new InfrastructureComponentSummary();

    /**
     * make an api call and load Component detail data
     *
     * @memberof ComponentDetail
     */
    loadData() {
        this.selectedDate = (this.date === null) ? undefined : DateTime.fromISO(this.date);
        this.service
            .getComponentDetailData(decodeURIComponent(this.id), this.selectedDate)
            .then(response => {
                if (response) {
                    this.data = response;
                    let index = this.data.scoreHistory.findIndex(x=>x.recordedAt.startsWith(this.date));
                    if(index<0) { index = 0; }
                    this.selectedDate = DateTime.fromISO(this.data.scoreHistory[index].recordedAt);
                    this.selectedScore = this.data.scoreHistory[index].score;
                    this.$emit('dateChanged', this.selectedDate.toISODate())
                    this.$emit('componentChanged', this.data.component)
                    this.loaded = true;
                    this.$forceUpdate();
                }
            })
            .catch(()=> { this.loadFailed = true; });
    }

    /**
     * return series for area chart
     *
     * @returns
     * @memberof ComponentDetail
     */
    getAreaSeries() {
        return [{ data: this.data.scoreHistory.map((item)=> ({ x: item.recordedAt.split('T')[0] , y: item.score })).reverse() }]
    }

    /**
     * return options for area chart
     *
     * @returns {ApexCharts.ApexOptions}
     * @memberof ComponentDetail
     */
    getAreaChartOptions() : ApexCharts.ApexOptions {
        return ChartService.AreaChartOptions("overviewchart", this.data.scoreHistory, [this.selectedDate!], [this.selectedScore], this.dayClicked);
    }

    /**
     * return series for pie chart
     *
     * @returns
     * @memberof ComponentDetail
     */
    getPieChartSeries() {
        return this.data.current.getSeries()
    }

    /**
     * return options for pie chart
     *
     * @returns {ApexCharts.ApexOptions}
     * @memberof ComponentDetail
     */
    getPieChartOptions() : ApexCharts.ApexOptions {
        return ChartService.PieChartOptions("pie-overall", this.data.current, this.pieClicked)
    }

    pieClicked(status: string) {
        let filterBy = btoa(`result=${status}&component=${this.data.component.name}`);
        router.push(`/overview-detail/${this.selectedDate!.toISODate()}/${filterBy}`); 
    }

    /**
     * handle click event on area chart
     * navigates to selected date
     *
     * @param {string} date
     * @param {string} component
     * @memberof ComponentDetail
     */
    dayClicked(date: string, component: string) {
        router.push('/component-detail/' + encodeURIComponent(this.data.component.id) + '/' + date);
    }

    /**
     * navigates to component history 
     *
     * @memberof ComponentDetail
     */
    goComponentHistory() {
        if (this.data.component) {
            router.push('/component-history/' + this.data.component.id);
        } else {
            router.push('/component-history/');
        }
    }

    /**
     * returns the url for image scan
     *
     * @param {string} imageTag
     * @returns
     * @memberof ComponentDetail
     */
    imageScanUrl(imageTag: string) {
        if(this.selectedDate === undefined) {
            this.selectedDate = DateTime.fromISO(this.data.scoreHistory[0].recordedAt);
        } 
        return '/image-detail/' + encodeURIComponent(imageTag) + '/' + this.selectedDate.toISODate();    
    }

    /**
     * returns short list of scans
     *
     * @readonly
     * @memberof ComponentDetail
     */
    get shortHistory() {
        return this.data.scoreHistory.slice(0, 5);
    }

    /**
     * returns results grouped by category
     *
     * @param {InfrastructureComponentSummary} data
     * @returns
     * @memberof ComponentDetail
     */
    getResultsByCategory(data: InfrastructureComponentSummary) { return MappingService.getResultsByCategory(this.data.checks); }

    /**
     * returns results grouped by collection
     *
     * @param {InfrastructureComponentSummary} data
     * @returns
     * @memberof ComponentDetail
     */
    getResultsByCollection(data: InfrastructureComponentSummary) { return MappingService.getResultsByCollection(data.checks, this.severityFilter); }

    /**
     * returns grade from score
     *
     * @param {number} score
     * @returns grade
     * @memberof ComponentDetail
     */
    getGrade(score: number) { return ScoreService.getGrade(score); }

    /**
     * returns metadata for category
     *
     * @param {string} category
     * @returns
     * @memberof ComponentDetail
     */
    getCategoryMeta(category: string) {
        let index = this.data.categorySummaries.findIndex(x => x.category === category);
        return (index > -1) ? this.data.categorySummaries[index].description : '';
    }

    /**
     * return class for scan history 
     *
     * @param {ScoreHistoryItem} scan
     * @returns
     * @memberof ComponentDetail
     */
    getHistoryClass(scan: ScoreHistoryItem) {
        if(scan.score === 0) {
            return 'history-not-scanned';
        }
        return scan.recordedAt.startsWith(this.selectedDate!.toISODate()) ? 'history-selected' : 'history';
    }

    /**
     * Watcher for date, emits dateChanged for breadcrumbs and loads data
     *
     * @private
     * @param {string} newValue
     * @memberof ComponentDetail
     */
    @Watch('date', { immediate: true })
    private onDateChanged(newValue: string) {
        this.selectedDate = DateTime.fromISO(newValue);
        this.$emit('dateChanged', this.selectedDate.toISODate())
        this.loadData();
    }

}