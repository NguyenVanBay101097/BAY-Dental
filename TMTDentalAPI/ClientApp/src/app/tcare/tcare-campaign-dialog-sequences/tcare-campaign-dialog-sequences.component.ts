import { Component, OnInit, ViewChild } from '@angular/core';
import { TcareService } from '../tcare.service';
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

  cell: any;
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
    if (this.cell.id)
      this.loadFormApi();
  }

  chossesMethodType(val) {
    this.type = val;
  }

  loadFormApi() {
    if (this.cell && this.cell.id) {
      this.tcareService.getTcareMessage(this.cell.id).subscribe(
        result => {
          this.type = result.methodType;
          this.formGroup.patchValue(result)
          if (result.methodType == "shedule")
            this.formGroup.get('sheduleDate').patchValue(new Date(result.sheduleDate));

        }
      )
    }
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
    if (this.formGroup.invalid || !this.cell || !this.cell.id)
      return false;

    var value = this.formGroup.value;
    if (this.campaignId)
      value.tCareCampaignId = this.campaignId;

    this.tcareService.updateTCareMessage(this.cell.id, value).subscribe(
      retult => {
        console.log('thành công');
        this.activeModal.close();
      }
    )
  }

}
