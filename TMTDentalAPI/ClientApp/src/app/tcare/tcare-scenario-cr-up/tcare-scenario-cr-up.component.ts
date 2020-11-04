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
  filterdChannelSocials: any[];
  submitted = false;
  textareaLength = 640;

  dayList: number[] = [];
  monthList: number[] = [];
  hourList: number[] = [];
  minuteList: number[] = [];
  daySource: number[] = [];
  monthSource: number[] = [];
  hourSource: number[] = [];
  minuteSource: number[] = [];

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
      type: ['auto_everyday', Validators.required],
      autoCustomType: null,
      customDay: null,
      customMonth: null,
      customHour: null,
      customMinute: null,
    });

    this.dobPrepare();


  }

  get nameControl() {
    return this.formGroup.get('name');
  }

  get channelSocialControl() {
    return this.formGroup.get('channelSocial');
  }

  get typeControl() {
    return this.formGroup.get('type');
  }

  get customTypeControl() {
    return this.formGroup.get('autoCustomType');
  }

  //Filter dữ liệu ngày tháng năm sinh
  handleFilterDay(value) {
    this.dayList = this.daySource.filter((s) => s.toString().toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  handleFilterMonth(value) {
    this.monthList = this.monthSource.filter((s) => s.toString().toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  handleFilterHour(value) {
    this.dayList = this.hourSource.filter((s) => s.toString().toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  handleFilterMinute(value) {
    this.monthList = this.minuteSource.filter((s) => s.toString().toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  onChangeType() {
    if (this.typeControl.value == "auto_custom") {
      this.formGroup.controls['autoCustomType'].setValue("custom1");
      this.formGroup.controls['customDay'].setValue(1);
      this.formGroup.controls['customMonth'].setValue(1);
      this.formGroup.controls['customHour'].setValue(0);
      this.formGroup.controls['customMinute'].setValue(0);
    } else {
      this.formGroup.controls['autoCustomType'].setValue(null);
      this.formGroup.controls['customDay'].setValue(null);
      this.formGroup.controls['customMonth'].setValue(null);
      this.formGroup.controls['customHour'].setValue(null);
      this.formGroup.controls['customMinute'].setValue(null);
    }
  }

  onChangeCustomType(){

      this.formGroup.controls['customDay'].setValue(1);
      this.formGroup.controls['customMonth'].setValue(1);
      this.formGroup.controls['customHour'].setValue(0);
      this.formGroup.controls['customMinute'].setValue(0);    
  }

  dobPrepare() {
    this.daySource = this.TimeInit(1, 31);
    this.dayList = this.daySource;
    this.monthSource = this.TimeInit(1, 12);
    this.monthList = this.monthSource;
    this.hourSource = this.TimeInit(0, 23);
    this.hourList = this.hourSource;
    this.minuteSource = this.TimeInit(0, 59);
    this.minuteList = this.minuteSource;
  }

  TimeInit(begin: number, end: number) {
    var a = new Array();
    if (begin < end) {
      for (let i = begin; i <= end; i++) {
        a.push(i);
      }
    } else if (begin > end) {
      for (let i = begin; i >= end; i--) {
        a.push(i);
      }
    }

    return a;
  }


  loadData() {
    if (this.id) {
      this.tcareService.getScenario(this.id).subscribe(
        (result: any) => {
          this.scenario = result;
          this.formGroup.patchValue(this.scenario);
          if (result.channelSocial) {
            this.filterdChannelSocials = _.unionBy(this.filterdChannelSocials, result.channelSocial, 'id');
            if (result.channelSocial.type === 'zalo') {
              this.textareaLength = 2000;
            }
          }
          if (result.customDay) {
            this.formGroup.get("customDay").setValue(result.customDay);
          }

          if (result.customMonth) {
            this.formGroup.get("customMonth").setValue(result.customMonth);
          }

          if (result.customHour) {
            this.formGroup.get("customHour").setValue(result.customHour);
          }

          if (result.customMinute) {
            this.formGroup.get("customMinute").setValue(result.customMinute);
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
    debugger
    var value = this.formGroup.value;
    value.channelSocialId = value.channelSocial ? value.channelSocial.id : null;
    value.type = value.type;
    value.autoCustomType = this.typeControl.value != "auto_custom" ? null : value.autoCustomType;
    value.customDay = this.typeControl.value != "auto_custom" ? null : parseInt(value.customDay);
    value.customMonth = this.typeControl.value != "auto_custom" ? null : parseInt(value.customMonth);
    value.customHour = this.typeControl.value != "auto_custom" ? null : parseInt(value.customHour);
    value.customMinute = this.typeControl.value != "auto_custom" ? null : parseInt(value.customMinute);

    if(this.id){
      this.tcareService.updateScenario(this.scenario.id, value).subscribe(
        () => {
          this.notificationService.show({
            content: "Lưu thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
  
         
        }
      )
    }else{
      this.tcareService.createScenario(value).subscribe(
        (result: any) => {
          this.router.navigate(['tcare/scenarios/form'], {
            queryParams: {
              id: result['id']
            },
          });
          this.loadData();
        }
      )
    }

  }

  actionNext(data) {
    if (data.graphXml) {
      this.campaign.graphXml = data.graphXml;
    }

    if (data.sheduleStartType) {
      this.campaign.sheduleStartType = data.sheduleStartType;
      this.campaign.sheduleStartNumber = data.sheduleStartNumber;
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
            content: "Chạy chiến dịch thành công!.",
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
    if (!this.id) {

      this.submitted = true;
      if (this.formGroup.invalid) {
        return false;
      }

      var value = this.formGroup.value;
      value.channelSocialId = value.channelSocial ? value.channelSocial.id : null;
      value.type = value.type;
      value.autoCustomType = this.typeControl.value != "auto_custom" ? null : value.autoCustomType;
      value.customDay = this.typeControl.value != "auto_custom" ? null : parseInt(value.customDay);
      value.customMonth = this.typeControl.value != "auto_custom" ? null : parseInt(value.customMonth);
      value.customHour = this.typeControl.value != "auto_custom" ? null : parseInt(value.customHour);
      value.customMinute = this.typeControl.value != "auto_custom" ? null : parseInt(value.customMinute);
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
    } else {
      this.openCreateCampaignDialog(this.id);
    }


  }

  openCreateCampaignDialog(id) {
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

  onSelectChannel(e) {
    if (e) {
      if (e.type === 'facebook') {
        this.textareaLength = 640;
      } else if (e.type === 'zalo') {
        this.textareaLength = 2000;
      }
    }
  }

}
