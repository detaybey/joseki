import { Component, Vue, Prop } from "vue-property-decorator";

import { DataService } from '@/services/';
import { ImageScanDetailModel, InfrastructureComponent } from '@/models';
import { DateTime } from 'luxon';

/**
 * Image detail is a view to display the details of an image scan
 *
 * @export
 * @class ImageDetail
 * @extends {Vue}
 */
@Component
export default class ImageDetail extends Vue {
  @Prop()
  imageid!: string;

  @Prop({ default: null })
  date!: string;

  @Prop()
  component?: InfrastructureComponent

  loaded: boolean = false;
  loadFailed: boolean = false;
  service: DataService = new DataService();
  data: ImageScanDetailModel = new ImageScanDetailModel();

  /**
   * Makes an api call to get the image scan result data
   *
   * @memberof ImageDetail
   */
  created() {
    this.loadData();
    this.$emit('dateChanged', DateTime.fromISO(this.date!).toISODate())
    if(this.component) {
      this.$emit('componentChanged', this.component);
    }
  }

  loadData() {
    this.service
        .getImageScanResultData(this.imageid, this.date)
        .then(response => {
          if (response) {
            this.data = response;
            this.loaded = true;
          }
        })
        .catch(()=> { this.loadFailed = true; });
  }
}
