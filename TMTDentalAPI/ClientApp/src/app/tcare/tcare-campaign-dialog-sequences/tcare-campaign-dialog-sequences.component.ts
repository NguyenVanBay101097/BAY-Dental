import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { TcareService, TCareMessageDisplay } from '../tcare.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { FacebookPagePaged } from 'src/app/socials-channel/facebook-page-paged';
import { FacebookPageService, ChannelSocial } from 'src/app/socials-channel/facebook-page.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { DatePipe } from '@angular/common';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-tcare-campaign-dialog-sequences',
  templateUrl: './tcare-campaign-dialog-sequences.component.html',
  styleUrls: ['./tcare-campaign-dialog-sequences.component.css'],
  host: {
    '(document:keypress)': 'handleKeyboardEvent($event)'
  }
})
export class TcareCampaignDialogSequencesComponent implements OnInit {

  @ViewChild('channelSocialCbx', { static: true }) channelSocialCbx: ComboBoxComponent;
  model: any;
  formGroup: FormGroup;
  filterdChannelSocials: ChannelSocial[] = [];
  submited = false;
  title: string;
  showPluginTextarea: boolean = false;
  selectArea_start: number;
  selectArea_end: number;
  audience_filter: any;
  showAudienceFilter: boolean = false;
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private facebookPageService: FacebookPageService,
    private intlService: IntlService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      // channelSocialId: ['', Validators.required],
      content: ['', Validators.required],
      methodType: ['interval', Validators.required],
      intervalNumber: [0],
      intervalType: ['minutes'],
      sheduleDate: null,
      // channelType: ['fixed', Validators.required]
    });

    // this.channelSocialCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.channelSocialCbx.loading = true)),
    //   switchMap(value => this.searchSocialChannel(value))
    // ).subscribe((result: any) => {
    //   this.filterdChannelSocials = result.items;
    //   this.channelSocialCbx.loading = false;
    // });

    // this.loadSocialChannel();

    if (this.model) {
      var tmp = Object.assign({}, this.model);
      tmp.intervalNumber = parseInt(this.model.intervalNumber) || 0;
      tmp.sheduleDate = this.model.sheduleDate ? new Date(this.model.sheduleDate) : null;
      this.formGroup.patchValue(tmp);
    }
  }

  // loadSocialChannel() {
  //   this.searchSocialChannel().subscribe((result: any) => {
  //     this.filterdChannelSocials = result.items;
  //   });
  // }

  get methodTypeValue() {
    return this.formGroup.get('methodType').value;
  }

  get contentControl() {
    return this.formGroup.get('content');
  }

  get channelSocialIdControl() {
    return this.formGroup.get('channelSocialId');
  }

  searchSocialChannel(q?: string) {
    var val = new FacebookPagePaged();
    val.search = q || '';
    return this.facebookPageService.getPaged(val);
  }

  onSave() {
    this.submited = true;
    if (!this.formGroup.valid) {
      return false;
    }
    var value = this.formGroup.value;
    value.intervalNumber = value.intervalNumber ? value.intervalNumber + '' : 0;
    value.sheduleDate = value.sheduleDate ? this.intlService.formatDate(value.sheduleDate, 'yyyy-MM-ddTHH:mm:ss') : '';
    this.activeModal.close(value);
  }

  selectArea(event) {
    this.selectArea_start = event.target.selectionStart;
    this.selectArea_end = event.target.selectionEnd;
  }

  getLimitText() {
    var limit = 640;
    var text = this.formGroup.get('content').value;
    if (text) {
      return limit - text.length;
    } else {
      return limit;
    }
  }

  addContentPluginTextarea(value) {
    if (this.formGroup.value.content) {
      this.formGroup.patchValue({
        content: this.formGroup.value.content.slice(0, this.selectArea_start) + value + this.formGroup.value.content.slice(this.selectArea_end)
      });
      this.selectArea_start = this.selectArea_start + value.length;
      this.selectArea_end = this.selectArea_start;
    } else {
      this.formGroup.patchValue({
        content: value
      });
    }
  }
  showEmoji() {
    this.showPluginTextarea = true;
  }
  
  hideEmoji() {
    this.showPluginTextarea = false;
  }
}
