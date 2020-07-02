import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { TCareScenarioDisplay, TcareService, TCareCampaignDisplay } from '../tcare.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TcareCampaignCreateDialogComponent } from '../tcare-campaign-create-dialog/tcare-campaign-create-dialog.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { load, IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-tcare-scenario-cr-up',
  templateUrl: './tcare-scenario-cr-up.component.html',
  styleUrls: ['./tcare-scenario-cr-up.component.css']
})
export class TcareScenarioCrUpComponent implements OnInit {
  id: string;
  formGroup: FormGroup;
  title = "Kịch bản"
  campaign: TCareCampaignDisplay;
  campaignId: string;
  scenario: TCareScenarioDisplay;
  constructor(
    private fb: FormBuilder,
    private activeRoute: ActivatedRoute,
    private tcareService: TcareService,
    private modalService: NgbModal,
    private intlService: IntlService,
    private notificationService: NotificationService,
    private router: Router
  ) { }

  ngOnInit() {
    this.id = this.activeRoute.params["_value"].id;
    if (this.id) {
      this.loadData()
    }
    this.scenario = new TCareScenarioDisplay();
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
    });

    this.tcareService.actionNext.subscribe(
      value => {
        if (value == "load") {
          this.loadData();
        }
      }
    )
  }

  get nameControl() {
    return this.formGroup.get('name');
  }

  loadData() {
    this.tcareService.getScenario(this.id).subscribe(
      result => {
        this.scenario = result;
        this.formGroup.patchValue(this.scenario);
        console.log(this.scenario);
        if (this.scenario && this.scenario.campaigns && !this.campaign && this.scenario.campaigns.length > 0)
          this.campaign = this.scenario.campaigns[0];

      }
    )
  }

  onSave() {
    if (this.formGroup.invalid)
      return false;
    var value = this.formGroup.value;
    this.tcareService.updateScenario(this.scenario.id, value).subscribe(
      () => {
        this.router.navigateByUrl('tcare-scenarios');
      }
    )
  }

  changeCheckedCampaign(e, campaign) {
    e.stopPropagation();
    if (campaign.active) {
      var value = [];
      value.push(this.campaign.id);
      this.tcareService.actionStopCampaign(value).subscribe(
        () => {
          this.campaign.active = false;
          this.tcareService.actionNext.next(this.campaign);
          this.notificationService.show({
            content: "Dừng kịch bản thành công!.",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
        }
      )
    } else {
      var val = {
        id: campaign.id,
        sheduleStart: this.intlService.formatDate(campaign.sheduleStart, "yyyy-MM-ddTHH:mm:ss")
      }
      this.tcareService.actionStartCampaign(val).subscribe(
        () => {
          this.campaign.active = true;
          this.tcareService.actionNext.next(this.campaign);
          this.notificationService.show({
            content: "Chạy kịch bản thành công!.",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
        }
      )
    }
  }



  editCampaign(campaign) {
    this.campaign = campaign;
    this.tcareService.actionNext.next(this.campaign);
  }

  addCampaign() {
    let modalRef = this.modalService.open(TcareCampaignCreateDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm chiến dịch mới';
    modalRef.componentInstance.scenarioId = this.id;
    modalRef.result.then(result => {
      if (result) {
        this.campaign = result
        this.scenario.campaigns.push(this.campaign);
        this.tcareService.actionNext.next(this.campaign);
      }

    }, () => {
    });

  }

  removeCampaign(item) {
    if (item.active) {
      this.notificationService.show({
        content: "Chiến dịch đang chạy bạn không thể xóa.",
        hideAfter: 3000,
        position: { horizontal: "center", vertical: "top" },
        animation: { type: "fade", duration: 400 },
        type: { style: "error", icon: true },
      });
    } else {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Xóa: ' + this.title;

      modalRef.result.then(() => {
        this.tcareService.delete(item.id).subscribe(() => {
          if (item.id == this.campaign.id) {
            this.campaign = null;
          }
          this.loadData();
          setTimeout(() => {
            if (this.campaign)
              this.tcareService.actionNext.next(this.campaign);
          }, 100);
        });
      });
    }

  }

}
