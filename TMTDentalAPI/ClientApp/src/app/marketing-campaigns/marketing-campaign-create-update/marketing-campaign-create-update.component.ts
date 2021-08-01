import { Component, OnInit } from '@angular/core';
import { MarketingCampaignLineDialogComponent } from '../marketing-campaign-line-dialog/marketing-campaign-line-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { MarketingCampaignService, MarketingCampaign, MarketingCampaignActivity } from '../marketing-campaign.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-marketing-campaign-create-update',
  templateUrl: './marketing-campaign-create-update.component.html',
  styleUrls: ['./marketing-campaign-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class MarketingCampaignCreateUpdateComponent implements OnInit {
  campaign: MarketingCampaign;
  campaignActivity: MarketingCampaignActivity;

  constructor(private fb: FormBuilder, private modalService: NgbModal, private route: ActivatedRoute,
    private router: Router, private marketingCampaignService: MarketingCampaignService,
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.defaultCampaign();
    this.routeActive();
  }

  defaultCampaign() {
    this.campaign = new MarketingCampaign();
    //this.campaign.id = '';
    this.campaign.name = '';
    this.campaign.state = 'draft';
    this.campaign.activities = [];
  }

  defaultCampaignActivity() {
    this.campaignActivity = new MarketingCampaignActivity();
    //this.campaignActivity.id = '';
    this.campaignActivity.name = '';
    this.campaignActivity.condition = '';
    this.campaignActivity.daysNoSales = 0;
    this.campaignActivity.activityType = 'facebook';
    this.campaignActivity.content = '';
    this.campaignActivity.intervalType = 'hours';
    this.campaignActivity.intervalNumber = 1;
    this.campaignActivity.triggerType = '';
    this.campaignActivity.everyDayTimeAt = '';
  }

  showCreateUpdateLineModal(item: any, index) {
    let modalRef = this.modalService.open(MarketingCampaignLineDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    if (item) {
      modalRef.componentInstance.title = 'Chỉnh sửa hoạt động';
      modalRef.componentInstance.item = item;
    } else {
      modalRef.componentInstance.title = 'Thêm hoạt động mới';
      this.defaultCampaignActivity()
      modalRef.componentInstance.item = this.campaignActivity;
    }
    modalRef.result.then((result) => {
      if (result) {
        if (item) {
          this.campaign.activities[index] = result;
        } else {
          this.campaign.activities.push(result);
        }
        modalRef.close();
      }
    }, (reason) => {
    });
  }

  deleteLineModal(index) {
    this.campaign.activities.splice(index, 1);
  }

  cancelCampaign() {
    this.router.navigate(['/marketing-campaigns']);
  }
  onStart() {
    this.campaign.state = 'running';
  }
  onStop() {
    this.campaign.state = 'stopped';
  }

  routeActive() {
    this.route.queryParamMap.pipe(
      switchMap((params: ParamMap) => {
        this.campaign.id = params.get("id");
        if (this.campaign.id) {
          return this.marketingCampaignService.get(this.campaign.id);
        } else {
          //return this.marketingCampaignService.defaultGet();
          return [];
        }
      })).subscribe(result => {
        if (result) {
          this.campaign = result;
        }
      });
  }

  onSave() {
    if (this.campaign.name.trim() !== '') {
      if (this.campaign.id) {
        this.marketingCampaignService.update(this.campaign.id, this.campaign).subscribe(() => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          //this.router.navigate(['/marketing-campaigns']);
        });
      } else {
        this.marketingCampaignService.create(this.campaign).subscribe((result: any) => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.router.navigate(['/marketing-campaigns/form'], { queryParams: { id: result.id } });
        });
      }

    } else {
      this.notificationService.show({
        content: 'Tên không được trống',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
    }
  }
}
