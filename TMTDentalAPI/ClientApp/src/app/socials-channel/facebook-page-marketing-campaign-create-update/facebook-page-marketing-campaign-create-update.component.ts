import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FacebookPageMarketingActivityDialogComponent } from '../facebook-page-marketing-activity-dialog/facebook-page-marketing-activity-dialog.component';
import { MarketingCampaignService } from 'src/app/marketing-campaigns/marketing-campaign.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { MarketingCampaignActivitiesService } from '../marketing-campaign-activities.service';

@Component({
  selector: 'app-facebook-page-marketing-campaign-create-update',
  templateUrl: './facebook-page-marketing-campaign-create-update.component.html',
  styleUrls: ['./facebook-page-marketing-campaign-create-update.component.css']
})
export class FacebookPageMarketingCampaignCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  campaign: any;
  activities: any;

  constructor(private fb: FormBuilder, private modalService: NgbModal,
    private marketingCampaignService: MarketingCampaignService,
    private notificationService: NotificationService,
    private router: Router, private route: ActivatedRoute, 
    private marketingCampaignActivitiesService: MarketingCampaignActivitiesService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      facebookPageId: null,
    });

    this.campaign = new Object();

    this.route.queryParamMap.pipe(
      switchMap((params: ParamMap) => {
        this.id = params.get("id");
        if (this.id) {
          return this.marketingCampaignService.get(this.id);
        } else {
          return this.marketingCampaignService.defaultGet();
        }
      })).subscribe((result: any) => {
        this.campaign = result;
        this.formGroup.patchValue(result);
        this.activities = this.campaign.activities;
        // console.log("this.campaign", this.campaign);
        // console.log("this.formGroup", this.formGroup);
        // console.log("this.activities", this.activities);  //
      });
  }

  loadRecord() {
    if (this.id) {
      this.marketingCampaignService.get(this.id).subscribe((result: any) => {
        this.campaign = result;
        this.formGroup.patchValue(result);
        this.activities = this.campaign.activities;
      });
    }
  }

  addActivity() {
    if (this.formGroup.value.name) {
      var val = {
        name: this.formGroup.value.name,
        facebookPageId: this.formGroup.value.facebookPageId
      }
      if (!this.id) {
        this.marketingCampaignService.create(val).subscribe((result: any) => {
          this.router.navigate([this.router.url], { queryParams: { id: result.id } });
        });
      }
  
      let modalRef = this.modalService.open(FacebookPageMarketingActivityDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Thêm hoạt động';
      modalRef.componentInstance.campaignId = this.id;
  
      modalRef.result.then(result => {
        if (result === "loading") {
          if (this.id) {
            this.marketingCampaignService.get(this.id).subscribe((result: any) => {
              this.campaign = result;
              this.formGroup.patchValue(result);
              this.activities = this.campaign.activities;
            });
          }
        }
      }, () => {
      });
    } else {
      this.notificationService.show({
        content: 'Tên chiến dịch không được để trống',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
    }
  }

  editActivity(activity) {
    if (this.clickedDeleteActivity === true) {
      this.clickedDeleteActivity = false;
      return;
    }
    let modalRef = this.modalService.open(FacebookPageMarketingActivityDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa hoạt động';
    modalRef.componentInstance.campaignId = this.id;
    modalRef.componentInstance.activityId = activity.id;
    modalRef.result.then(result => {
      if (result === "loading") {
        if (this.id) {
          this.marketingCampaignService.get(this.id).subscribe((result: any) => {
            this.campaign = result;
            this.formGroup.patchValue(result);
            this.activities = this.campaign.activities;
            // console.log("this.campaign", this.campaign);
            // console.log("this.formGroup", this.formGroup);
            // console.log("this.activities", this.activities);
          });
        }
      }
    }, () => {
    });
  }
  clickedDeleteActivity: boolean = false;
  deleteActivity(activity_id) {
    this.clickedDeleteActivity = true;
    var alert = confirm("Bạn có muốn xóa hoạt động này không?");
    if (alert == true) {
      console.log("activity_id", activity_id);
      this.marketingCampaignActivitiesService.delete(activity_id).subscribe((result: any) => {
        this.marketingCampaignService.get(this.id).subscribe((result: any) => {
          this.campaign = result;
          this.formGroup.patchValue(result);
          this.activities = this.campaign.activities;
          this.notificationService.show({
            content: 'Xóa hoạt động thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        });
      });
    }
  }

  getTrigger(activity: FormGroup) {
    var intervalType = activity.get('intervalType').value;
    var intervalNumber = activity.get('intervalNumber').value;
    if (intervalType == 'hours') {
      return `${intervalNumber} Giờ`;
    } else if (intervalType == 'minutes') {
      return `${intervalNumber} Phút`;
    } else if (intervalType == 'weeks') {
      return `${intervalNumber} Tuần`;
    } else if (intervalType == 'months') {
      return `${intervalNumber} Tháng`;
    } else {
      return `${intervalNumber} Ngày`;
    }
  }

  startCampaign() {
    if (!this.formGroup.valid) {
      return false;
    }

    if (this.id) {
      this.marketingCampaignService.actionStartCampaign([this.id]).subscribe(() => {
        this.loadRecord();
      });
    } else {
      var value = this.formGroup.value;
      this.marketingCampaignService.create(value).subscribe((result: any) => {
        this.marketingCampaignService.actionStartCampaign([result.id]).subscribe(() => {
          this.router.navigate([this.router.url], { queryParams: { id: result.id } });
        });
      });
    }
  }

  stopCampaign() {
    if (this.id) {
      this.marketingCampaignService.actionStopCampaign([this.id]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    if (!this.id) {
      var value = this.formGroup.value;
      console.log("this.campaign", this.campaign);
      console.log("this.formGroup", this.formGroup);
      console.log("this.activities", this.activities);
      this.marketingCampaignService.create(value).subscribe((result: any) => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });

        this.router.navigate([this.router.url], { queryParams: { id: result.id } });
      });
    } else {
      var value = this.formGroup.value;
      this.marketingCampaignService.update(this.id, value).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    }
  }

  createNew() {
    var url = this.router.url.split('?')[0];
    this.router.navigate([url]);
  }

  convertIntervalType(intervalType) {
    switch (intervalType) {
      case "minutes":
        return "Phút";
      case "hours":
        return "Giờ";
      case "days":
        return "Ngày";
      case "weeks":
        return "Tuần";
      case "months":
        return "Tháng";
    }
  }

  clickSort() {
    /*
    var items: any[] = [];
    items = this.activities;
    var dict = {};
    for(var i = 0; i < items.length; i++) {
      var item = items[i];
      if (item.parentId) {
        if (!dict[item.parentId]) {
          dict[item.parentId] = [];
          dict[item.parentId].push(item.id);
        } else {
          dict[item.parentId].push(item.id);
        }
      }
    }
    console.log(dict);
    */
        // var index_parentId = this.activities.findIndex(x => x.parentId == activities_copy[i].parentId);
        // var temp = activities_copy[i];
        // activities_copy.splice(i, 1);
        // activities_copy.splice(index_parentId + 1, 0, temp);
    /*
    var activities_sort = [];
    var activities_length = this.activities.length;
    for (let i = 0; i < activities_length; i++) {
      if (this.activities[i].triggerType === "begin") {
        activities_sort.push(this.activities[i]);
      }
    }
    var index_parentId;
    for (let i = 0; i < activities_length; i++) {
      if (this.activities[i].triggerType !== "begin") {
        index_parentId = activities_sort.findIndex(x => x.id == this.activities[i].parentId);
        if (index_parentId >= 0) {
          activities_sort.splice(index_parentId + 1, 0, this.activities[i]);
        }
      }
    }
    console.log(activities_sort);
    this.activities = activities_sort;
    */
    //
    var activities_sort = [];
    var activities_length = this.activities.length;
    var i = 0;
    var index_parentId;
    var index_parentId_sort;
    while (i < activities_length) {
      console.log("i", i);
      if (this.activities[i].triggerType === "begin") {
        activities_sort.push(this.activities[i]);
        this.activities.splice(i, 1);
        activities_length = this.activities.length;
        i = i - 1;
      } else {
        index_parentId = this.activities.findIndex(x => x.id == this.activities[i].parentId);
        index_parentId_sort = activities_sort.findIndex(x => x.id == this.activities[i].parentId);
        if (index_parentId_sort >= 0) {
          activities_sort.splice(index_parentId_sort + 1, 0, this.activities[i]);
          this.activities.splice(i, 1);
          activities_length = this.activities.length;
          i = i - 1;
        } else if (index_parentId < 0) {
          
        }
      }
      console.log(this.activities);
      i = i + 1;
      // if (i >= activities_length && activities_length > 0) {
      //   i = 0;
      // } 
      if (activities_length == 0) {
        console.log(activities_sort);
        this.activities = activities_sort;
      }
    }
  }
}
