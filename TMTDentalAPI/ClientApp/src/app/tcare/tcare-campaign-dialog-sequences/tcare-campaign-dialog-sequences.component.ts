import { Component, OnInit, ViewChild } from '@angular/core';
import { TcareService, TCareMessageDisplay } from '../tcare.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { FacebookPagePaged } from 'src/app/socials-channel/facebook-page-paged';
import { FacebookPageService, ChannelSocial } from 'src/app/socials-channel/facebook-page.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-tcare-campaign-dialog-sequences',
  templateUrl: './tcare-campaign-dialog-sequences.component.html',
  styleUrls: ['./tcare-campaign-dialog-sequences.component.css'],
})
export class TcareCampaignDialogSequencesComponent implements OnInit {

  @ViewChild('channelSocialCbx', { static: true }) channelSocialCbx: ComboBoxComponent;

  model: TCareMessageDisplay;
  campaignId: string;
  formGroup: FormGroup;
  loading = false;
  limit = 10;
  offset = 0;
  filterdChannelSocials: ChannelSocial[] = [];
  type: string;
  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private tcareService: TcareService,
    private facebookPageService: FacebookPageService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      channelSocialId: ['', Validators.required],
      content: ['', Validators.required],
      methodType: ['interval', Validators.required],
      intervalNumber: [0],
      intervalType: ['minutes'],
      sheduleDate: new Date(),
      channelType: ['priority', Validators.required]
    });
    this.type = this.formGroup.get('methodType').value;
    this.channelSocialCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.channelSocialCbx.loading = true)),
      switchMap(value => this.searchSocialChannel(value))
    ).subscribe(result => {
      this.filterdChannelSocials = result.items;
      this.channelSocialCbx.loading = false;
    });

    this.loadSocialChannel();
    if (this.model) {
      if (this.model.methodType)
        this.type = this.model.methodType;

      this.loadFormApi();
    }

  }

  chossesMethodType(val) {
    this.type = val;
  }

  loadFormApi() {
    this.formGroup.patchValue(this.model);
  }

  loadSocialChannel() {
    this.searchSocialChannel().subscribe(
      result => {
        this.filterdChannelSocials = result.items;
      }
    )
  }

  searchSocialChannel(q?: string) {
    var val = new FacebookPagePaged();
    val.limit = this.limit;
    val.offset = this.offset;
    val.search = q || '';
    return this.facebookPageService.getPaged(val);
  }

  onSave() {
    this.model = this.formGroup.value;
    this.activeModal.close(this.model);
  }

}
