import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import router from '@/router';
import { DateTime } from 'luxon';

import { DataService, ScoreService, ChartService } from '@/services/';
import { InfrastructureOverview, ScoreHistoryItem, InfrastructureComponent } from '@/models';

/**
 * Overview is the landing page and displays the 
 * current status of overall infrastructure.
 *
 * @export
 * @class Overview
 * @extends {Vue}
 */
@Component
export default class Overview extends Vue {

    @Prop({ default: null })
    date!: string;

    selectedScore: number = 0;
    selectedDate?: DateTime = undefined;
    loaded: boolean = false;
    loadFailed: boolean = false;
    service: DataService = new DataService();
    data!: InfrastructureOverview;
    noScanHistory: boolean = false;

    /**
     * Make an api call for getting the general overview data
     *
     * @memberof Overview
     */
    loadData() {
        this.loadFailed = false;
        this.selectedDate = (this.date === null) ? undefined : DateTime.fromISO(this.date);
        this.service
            .getGeneralOverviewData(this.selectedDate)
            .then(response => {
                if (response) {
                    this.data = response;
                    if (this.data.overall.scoreHistory.length <= 1) {
                        this.noScanHistory = true;
                        this.selectedDate = DateTime.fromISO(this.data.overall.date);
                        this.$emit('dateChanged', this.selectedDate.toISODate())
                        setTimeout(this.loadData, 10* 1000);
                    }else {
                        if(this.data.components.length>0) {
                            let index = this.data.overall.scoreHistory.findIndex(x=>x.recordedAt.startsWith(this.date));
                            if(index<0) { index = 0; }
                            this.selectedDate = DateTime.fromISO(this.data.overall.scoreHistory[index].recordedAt);
                            this.selectedScore = this.data.overall.scoreHistory[index].score;
                            this.$emit('dateChanged', this.selectedDate.toISODate())
                            this.$emit('componentChanged', this.data.overall.component)    
                        }else {
                            this.selectedScore = 0;
                            this.selectedDate = DateTime.utc();
                        }
                    }
                    this.loaded = true;
                    this.$forceUpdate();
                }
            })
            .catch((error)=> { 
                console.log(error);
                this.loadFailed = true; 
            });
    }
 
    /**
     * Returns series data for overall area chart at top right
     *
     * @returns
     * @memberof Overview
     */
    getAreaSeries() {
        if(this.data.components.length == 0) {
            return [{ data: [{x: this.selectedDate, y: 0}]}]
        }
        return ScoreHistoryItem.getInterpolatedThresholdSeries(this.data.overall.scoreHistory);        
    }

    /**
     * Returns options for area chart at top right
     *
     * @returns {ApexCharts.ApexOptions}
     * @memberof Overview
     */
    getAreaChartOptions() : ApexCharts.ApexOptions {
        return ChartService.AreaChartOptions("overviewchart", this.data.overall.scoreHistory, [this.selectedDate!], [this.selectedScore], this.dayClicked);
    }

    /**
     * Returns series data for pie chart in the middle
     *
     * @returns
     * @memberof Overview
     */
    getPieChartSeries() {
        return this.data.overall.current.getSeries()
    }

    /**
     * Returns options for pie chart in the middle
     *
     * @returns {ApexCharts.ApexOptions}
     * @memberof Overview
     */
    getPieChartOptions() : ApexCharts.ApexOptions {
        return ChartService.PieChartOptions("pie-overall", this.data.overall.current, this.pieClicked)
    }

    /**
     * Handle click segment on pie chart in the middle.
     *
     * @param {string} status
     * @memberof Overview
     */
    pieClicked(status: string) {
        let filterBy = btoa(`result=${status}`);
        router.push(`/overview-detail/${this.selectedDate!.toISODate()}/${filterBy}`); 
    }

    /**
     * Handle a click on top right chart, that navigates to selected date
     *
     * @param {string} date
     * @memberof Overview
     */
    dayClicked(date: string) {
        this.selectedDate = DateTime.fromISO(date);
        router.push('/overview/' + date);
        this.$forceUpdate();
    }

    /**
     * Navigates to component-history view using component parameter
     *
     * @param {InfrastructureComponent} component
     * @memberof Overview
     */
    goComponentHistory(component: InfrastructureComponent) {
        if (component) {
            router.push('/component-history/' + component.id);
        } else {
            router.push('/component-history/');
        }
    }

    /**
     * Returns short list of scan history
     *
     * @returns
     * @memberof Overview
     */
    getShortHistory() { return this.data.overall.scoreHistory.slice(0, 7); }
    
    /**
     * Returns grade from score
     *
     * @param {number} score
     * @returns grade
     * @memberof Overview
     */
    getGrade(score: number) { return ScoreService.getGrade(score); }

    /**
     * Returns number of clusters
     *
     * @returns
     * @memberof Overview
     */
    getClusters() { return this.data.components.filter(x => x.component.category === 'Kubernetes').length; }
    
    /**
     * Returns number of subscriptions
     *
     * @returns
     * @memberof Overview
     */
    getSubscriptions() { return this.data.components.filter(x => x.component.category === 'Azure Subscription').length; }
    
    /**
     * Returns class for rows on scan history
     *
     * @param {ScoreHistoryItem} scan
     * @returns
     * @memberof Overview
     */
    getHistoryClass(scan: ScoreHistoryItem) {
        return scan.recordedAt.startsWith(this.selectedDate!.toISODate()) ? 'history-selected' : 'history';
    }

    /**
     * Watcher for date, emits dateChanged for breadcrumbs and loads data
     *
     * @private
     * @param {string} newValue
     * @memberof Overview
     */
    @Watch('date', { immediate: true })
    private onDateChanged(newValue: string) {
        this.selectedDate = DateTime.fromISO(newValue);
        this.$emit('dateChanged', this.selectedDate.toISODate())
        this.loadData();
    }
}
