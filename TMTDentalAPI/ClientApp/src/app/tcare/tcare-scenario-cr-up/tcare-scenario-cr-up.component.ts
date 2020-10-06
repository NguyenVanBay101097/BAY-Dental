import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { TCareScenarioDisplay, TcareService, TCareCampaignDisplay } from '../tcare.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TcareCampaignCreateDialogComponent } from '../tcare-campaign-create-dialog/tcare-campaign-create-dialog.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { load, IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ChannelSocial, FacebookPageService } from 'src/app/socials-channel/facebook-page.service';
import { FacebookPagePaged } from 'src/app/socials-channel/facebook-page-paged';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import * as _ from 'lodash';

@Component({
  selector: 'app-tcare-scenario-cr-up',
  templateUrl: './tcare-scenario-cr-up.component.html',
  styleUrls: ['./tcare-scenario-cr-up.component.css']
})
export class TcareScenarioCrUpComponent implements OnInit {
  @ViewChild('channelSocialCbx', { static: true }) channelSocialCbx: ComboBoxComponent;
  id: string;
  formGroup: FormGroup;
  title = "Kịch bản"
  campaign: TCareCampaignDisplay;
  campaignId: string;
  dateStartCampaign: Date;
  scenario: TCareScenarioDisplay;
  filterdChannelSocials: any[] ;
  submitted = false;
  constructor(
    private fb: FormBuilder,
    private activeRoute: ActivatedRoute,
    private tcareService: TcareService,
    private modalService: NgbModal,
    private intlService: IntlService,
    private notificationService: NotificationService,
    private facebookPageService: FacebookPageService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.scenario = new TCareScenarioDisplay();
    this.route.queryParamMap.subscribe(params => {
      this.id = params.get('id');
      debugger
      if (!this.id) {
        this.loadData();
      } else {
        this.loadData();
      }

      this.channelSocialCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.channelSocialCbx.loading = true)),
        switchMap(value => this.searchSocialChannel(value))
      ).subscribe((result: any) => {
        this.filterdChannelSocials = result.items;
        this.channelSocialCbx.loading = false;
      });
  
      this.loadSocialChannel();
    });
    
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      channelSocialId: null,
      channelSocial: [null, Validators.required],
    });

   


  }

  get nameControl() {
    return this.formGroup.get('name');
  }

  get channelSocialControl() {
    return this.formGroup.get('channelSocial');
  }

  loadData() {
    if (this.id) {
      this.tcareService.getScenario(this.id).subscribe(
        (result: any) => {
          this.scenario = result;
          this.formGroup.patchValue(this.scenario);
          if (result.channelSocial) {
            this.filterdChannelSocials = _.unionBy(this.filterdChannelSocials, result.channelSocial, 'id');
          }
        }
      )
    }

  }

  loadSocialChannel() {
    this.searchSocialChannel().subscribe((result: any) => {
      this.filterdChannelSocials = result.items;
    });
    
  }

  searchSocialChannel(q?: string) {
    var val = new FacebookPagePaged();
    val.search = q || '';
    return this.facebookPageService.getPaged(val);
  }



  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid)
      return false;
    
    var value = this.formGroup.value;
    value.channelSocialId = value.channelSocial ? value.channelSocial.id : null;
    this.tcareService.updateScenario(this.scenario.id, value).subscribe(
      () => {
        this.notificationService.show({
          content: "Lưu thành công",
          hideAfter: 3000,
          position: { horizontal: "center", vertical: "top" },
          animation: { type: "fade", duration: 400 },
          type: { style: "success", icon: true },
        });

        this.loadData();
      }
    )
  }

  actionNext(data) {
    if (data.graphXml) {
      this.campaign.graphXml = data.graphXml;
    }

    if (data.sheduleStart) {
      this.campaign.sheduleStart = data.sheduleStart;
    }
  }

  timeChangeCampaign(event) {
    this.dateStartCampaign = event;
  }

  changeCheckedCampaign(e, campaign) {
    e.stopPropagation();
    if (campaign.active) {
      var value = [];
      value.push(this.campaign.id);
      this.tcareService.actionStopCampaign(value).subscribe(
        () => {
          campaign.active = false;
          this.notificationService.show({
            content: "Dừng kịch bản thành công!.",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
        }, () => {
        }
      )
    } else {
      var val = {
        id: campaign.id,
        sheduleStart: this.dateStartCampaign ? this.intlService.formatDate(this.dateStartCampaign, "yyyy-MM-ddTHH:mm:ss") : this.intlService.formatDate(campaign.sheduleStart, "yyyy-MM-ddTHH:mm:ss")
      }
      this.tcareService.actionStartCampaign(val).subscribe(
        () => {
          campaign.active = true;
          this.notificationService.show({
            content: "Chạy kịch bản thành công!.",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
        }, () => {
        }
      )
    }
  }



  editCampaign(campaign) {
    this.campaign = campaign;
  }

  addCampaign() {
    if(!this.id){

      this.submitted = true;
      if (this.formGroup.invalid) {
        return false;
      }

      var value = this.formGroup.value;
      value.channelSocialId = value.channelSocial ? value.channelSocial.id : null;
      this.tcareService.createScenario(value).subscribe(
        (result: any) => {          
          this.router.navigate(['tcare/scenarios/form'], {
            queryParams: {
              id: result['id']
            },
          });         
          this.openCreateCampaignDialog(result.id);
        }
      )
    }else{
      this.openCreateCampaignDialog(this.id);
    }

  
  }

  openCreateCampaignDialog(id){
    let modalRef = this.modalService.open(TcareCampaignCreateDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm chiến dịch mới';
    modalRef.componentInstance.scenarioId = id;
    modalRef.result.then(result => {
      if (result) {
        this.campaign = result
        this.scenario.campaigns.push(this.campaign);
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
      modalRef.componentInstance.title = 'Xóa chiến dịch';

      modalRef.result.then(() => {
        this.tcareService.delete(item.id).subscribe(() => {
          if (item.id == this.campaign.id) {
            this.campaign = this.scenario.campaigns[0];
          }
          this.loadData();
        });
      });
    }

  }

}
