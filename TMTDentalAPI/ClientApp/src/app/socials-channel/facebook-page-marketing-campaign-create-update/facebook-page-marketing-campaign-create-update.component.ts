import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FacebookPageMarketingActivityDialogComponent } from '../facebook-page-marketing-activity-dialog/facebook-page-marketing-activity-dialog.component';
import { MarketingCampaignService } from 'src/app/marketing-campaigns/marketing-campaign.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-facebook-page-marketing-campaign-create-update',
  templateUrl: './facebook-page-marketing-campaign-create-update.component.html',
  styleUrls: ['./facebook-page-marketing-campaign-create-update.component.css']
})
export class FacebookPageMarketingCampaignCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  campaign: any;

  constructor(private fb: FormBuilder, private modalService: NgbModal,
    private marketingCampaignService: MarketingCampaignService,
    private notificationService: NotificationService,
    private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      facebookPageId: null,
      activities: this.fb.array([]),
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
        if (result.activities) {
          this.activities.clear();
          result.activities.forEach(activity => {
            var g = this.fb.group(activity);
            if (activity.buttons) {
              g.setControl('buttons', this.fb.array(activity.buttons || []));
            }

            this.activities.push(g);
          });
        }
      });
  }

  loadRecord() {
    if (this.id) {
      this.marketingCampaignService.get(this.id).subscribe((result: any) => {
        this.campaign = result;
        this.formGroup.patchValue(result);
        if (result.activities) {
          this.activities.clear();
          result.activities.forEach(activity => {
            var g = this.fb.group(activity);
            if (activity.buttons) {
              g.setControl('buttons', this.fb.array(activity.buttons || []));
            }

            this.activities.push(g);
          });
        }
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
        console.log(result); // 
        //
        var g = this.fb.group(result);
        if (result.buttons) {
          g.setControl('buttons', this.fb.array(result.buttons || []));
        }
        console.log(g);
        this.activities.push(g);
        console.log("this.activities", this.activities); //
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

  editActivity(activity: FormGroup) {
    let modalRef = this.modalService.open(FacebookPageMarketingActivityDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa hoạt động';
    modalRef.componentInstance.activity = activity.value;
    modalRef.result.then(result => {
      var a = result as any;
      activity.patchValue(result);
      if (a.buttons) {
        activity.setControl('buttons', this.fb.array(a.buttons || []));
      }
    }, () => {
    });
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

  deleteActivity(index: number) {
    this.activities.removeAt(index);
  }

  get activities() {
    return this.formGroup.get('activities') as FormArray;
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
}
