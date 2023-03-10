import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { switchMap } from 'rxjs/operators';
import { MarketingCampaignService } from 'src/app/marketing-campaigns/marketing-campaign.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { FacebookPageMarketingActivityDialogComponent } from '../facebook-page-marketing-activity-dialog/facebook-page-marketing-activity-dialog.component';
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

  constructor(
    private fb: FormBuilder, private modalService: NgbModal,
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
        this.sortActivities();
      });
  }

  loadRecord() {
    if (this.id) {
      this.marketingCampaignService.get(this.id).subscribe((result: any) => {
        this.campaign = result;
        this.formGroup.patchValue(result);
        this.activities = this.campaign.activities;
        this.sortActivities();
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
          this.id = result.id;
          let modalRef = this.modalService.open(FacebookPageMarketingActivityDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
          modalRef.componentInstance.title = 'Th??m ho???t ?????ng';
          modalRef.componentInstance.campaignId = this.id;

          modalRef.result.then(result => {
            if (result === "loading") {
              this.loadRecord();
            }
          }, () => {
          });
        });
      } else {
        let modalRef = this.modalService.open(FacebookPageMarketingActivityDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'Th??m ho???t ?????ng';
        modalRef.componentInstance.campaignId = this.id;

        modalRef.result.then(result => {
          if (result === "loading") {
            this.loadRecord();
          }
        }, () => {
        });
      }
    } else {
      this.notificationService.show({
        content: 'T??n chi???n d???ch kh??ng ???????c ????? tr???ng',
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
    if (this.clickedAddChildActivity === true) {
      this.clickedAddChildActivity = false;
      return;
    }
    let modalRef = this.modalService.open(FacebookPageMarketingActivityDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a ho???t ?????ng';
    modalRef.componentInstance.campaignId = this.id;
    modalRef.componentInstance.activityId = activity.id;
    modalRef.result.then(result => {
      if (result === "loading") {
        this.loadRecord();
      }
    }, () => {
    });
  }
  clickedDeleteActivity: boolean = false;
  deleteActivity(activity_id) {
    this.clickedDeleteActivity = true;
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'X??a ho???t ?????ng';
    modalRef.componentInstance.body = 'B???n ch???c ch???n mu???n x??a?';
    modalRef.result.then(() => {
      // console.log("activity_id", activity_id);
      this.marketingCampaignActivitiesService.delete(activity_id).subscribe((result: any) => {
        this.loadRecord();
        this.notificationService.show({
          content: 'X??a ho???t ?????ng th??nh c??ng',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    });
  }
  clickedAddChildActivity: boolean = false;
  addChildActivity(activity, triggerType) {
    this.clickedAddChildActivity = true;
    if (!activity || !triggerType) {
      return;
    }
    let modalRef = this.modalService.open(FacebookPageMarketingActivityDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Th??m ho???t ?????ng";
    modalRef.componentInstance.campaignId = this.id;
    modalRef.componentInstance.parentId = activity.id;
    modalRef.componentInstance.triggerType = triggerType;
    modalRef.result.then(result => {
      if (result === "loading") {
        this.loadRecord();
      }
    }, () => {
    });
  }

  getTrigger(activity: FormGroup) {
    var intervalType = activity.get('intervalType').value;
    var intervalNumber = activity.get('intervalNumber').value;
    if (intervalType == 'hours') {
      return `${intervalNumber} Gi???`;
    } else if (intervalType == 'minutes') {
      return `${intervalNumber} Ph??t`;
    } else if (intervalType == 'weeks') {
      return `${intervalNumber} Tu???n`;
    } else if (intervalType == 'months') {
      return `${intervalNumber} Th??ng`;
    } else {
      return `${intervalNumber} Ng??y`;
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
          content: 'L??u th??nh c??ng',
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
          content: 'L??u th??nh c??ng',
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
        return "Ph??t";
      case "hours":
        return "Gi???";
      case "days":
        return "Ng??y";
      case "weeks":
        return "Tu???n";
      case "months":
        return "Th??ng";
    }
  }

  sortActivities() {
    var activities_sort = [];
    var activities_length = this.activities.length;
    var i = 0;
    var index_parentId;
    var index_parentId_sort;
    while (i < activities_length) {
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
          activities_sort.push(this.activities[i]);
          this.activities.splice(i, 1);
          activities_length = this.activities.length;
          i = i - 1;
        }
      }
      i = i + 1;
      if (i >= activities_length && activities_length > 0) {
        i = 0;
      }
      if (activities_length == 0) {
        this.activities = activities_sort;
        this.addMarginLeft();
        // console.log(this.activities);
      }
    }
  }

  addMarginLeft() {
    var index_parentId;
    for (let i = 0; i < this.activities.length; i++) {
      if (this.activities[i].triggerType === "begin") {
        this.activities[i].marginLeft = 0;
      } else {
        index_parentId = this.activities.findIndex(x => x.id == this.activities[i].parentId);
        if (index_parentId >= 0) {
          this.activities[i].marginLeft = this.activities[index_parentId].marginLeft + 90;
        } else {
          this.activities[i].marginLeft = 0;
        }
      }
    }
  }
}
