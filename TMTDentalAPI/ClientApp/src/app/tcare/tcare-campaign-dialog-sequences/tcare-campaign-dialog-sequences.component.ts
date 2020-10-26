import { Component, ElementRef, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { TcareService, TCareMessageDisplay } from '../tcare.service';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { FacebookPagePaged } from 'src/app/socials-channel/facebook-page-paged';
import { FacebookPageService, ChannelSocial } from 'src/app/socials-channel/facebook-page.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { DatePipe } from '@angular/common';
import { IntlService } from '@progress/kendo-angular-intl';
import { TCareMessageTemplatePaged, TCareMessageTemplateService } from '../tcare-message-template.service';
import { SaleCouponProgramPaged, SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { validate } from 'fast-json-patch';
import { TcareMessageTemplateCuDialogComponent } from '../tcare-message-template-cu-dialog/tcare-message-template-cu-dialog.component';

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
  @ViewChild('cbxMess', { static: true }) cbxMess: ComboBoxComponent;
  @ViewChild('couponCbx', { static: true }) couponCbx: ComboBoxComponent;
  @ViewChild('content', { static: false }) content: ElementRef;

  model: any;
  formGroup: FormGroup;
  filterdChannelSocials: ChannelSocial[] = [];
  submited = false;
  title: string;
  showPluginTextarea: boolean = false;
  selectArea_start: number = 0;
  selectArea_end: number = 0;
  audience_filter: any;
  showAudienceFilter: boolean = false;
  messageTemplates: any[];
  listCoupon: any;
  @Input()textareaLength = 640;
  mau: any;

  //cá nhân hóa
  tabs = [
    { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
    { name: 'Họ tên khách hàng', value: '{ho_ten_khach_hang}' },
    { name: 'Tên trang', value: '{ten_page}' },
    { name: 'Danh xưng khách hàng', value: '{danh_xung_khach_hang}' },
    { name: 'mã coupon', value: '{ma_coupon}' },
  ];

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private facebookPageService: FacebookPageService,
    private intlService: IntlService,
    private templateService: TCareMessageTemplateService,
    private saleCouponService: SaleCouponProgramService,
    private modalService : NgbModal

  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      // channelSocialId: ['', Validators.required],
      content: ['', Validators.required],
      methodType: ['interval', Validators.required],
      intervalNumber: [0],
      intervalType: ['minutes'],
      sheduleDate: null,
      channelType: ['fixed', Validators.required],
      isCoupon: false,
      couponProgramId: null
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
      tmp.isCoupon = Boolean(tmp.isCoupon);
      this.formGroup.patchValue(tmp);
    }

    this.cbxMess.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.cbxMess.loading = true)),
      switchMap(value => this.searchMess(value))
    ).subscribe((result: any) => {
      this.messageTemplates = result.items;
      this.cbxMess.loading = false;
    });

    this.loadMessageTemplate();

    this.couponCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.couponCbx.loading = true)),
      switchMap(value => this.searchCoupon(value))
    ).subscribe((result: any) => {
      this.listCoupon = result.items;
      console.log(result.items);
      this.couponCbx.loading = false;
    });

    this.loadCoupon();

  }
  searchCoupon(q?: string) {
    const val = new SaleCouponProgramPaged();
    val.search = q || '';
    val.active = true;
    val.programType = 'coupon_program';
    return this.saleCouponService.getPaged(val);
  }

  loadCoupon() {
    this.searchCoupon().subscribe((result: any) => {
      this.listCoupon = result.items;
    });
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

  get isCouponControl() { return this.formGroup.get('isCoupon'); }
  get couponProgramIdControl() { return this.formGroup.get('couponProgramId'); }

  searchSocialChannel(q?: string) {
    var val = new FacebookPagePaged();
    val.search = q || '';
    return this.facebookPageService.getPaged(val);
  }

  onSave() {
    debugger;
    this.submited = true;
    if (!this.formGroup.valid) {
      return false;
    }
    var value = this.formGroup.value;
    value.intervalNumber = value.intervalNumber ? value.intervalNumber + '' : 0;
    value.sheduleDate = value.sheduleDate ? this.intlService.formatDate(value.sheduleDate, 'yyyy-MM-ddTHH:mm:ss') : '';
    value.couponProgramId = value.couponProgramId ? value.couponProgramId : null;
    this.activeModal.close(value);
  }

  selectArea(event) {
    this.selectArea_start = event.target.selectionStart;
    this.selectArea_end = event.target.selectionEnd;
  }

  getLimitText() {
    const text = this.formGroup.get('content').value;
    if (text) {
      return this.textareaLength - text.length;
    } else {
      return this.textareaLength;
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

  onMessageTemplateSelect(e) {
    this.mau = e;
    if (!e) { return; }
    this.cbxMess.value = e;
    const templates = JSON.parse(e.content);
    this.formGroup.get('content').setValue(templates[0].text);
  }

  searchMess(q?: string) {
    const val = new TCareMessageTemplatePaged();
    val.search = q || '';
    return this.templateService.getPaged(val);
  }
  loadMessageTemplate() {
    this.searchMess().subscribe((res) => {
      this.messageTemplates = res.items;
    });
  }

  addToContent(value) {

    if (this.formGroup.value.content) {
      this.formGroup.patchValue({
        content: this.formGroup.value.content.slice(0, this.selectArea_start) + value + this.formGroup.value.content.slice(this.selectArea_end)
      });

    } else {
      this.formGroup.patchValue({
        content: value
      });
    }
    this.selectArea_start = this.selectArea_start + value.length;
    this.selectArea_end = this.selectArea_start;

    this.content.nativeElement.focus();
    this.content.nativeElement.selectionEnd = this.selectArea_end;
    this.content.nativeElement.selectionStart = this.selectArea_start;
  }

  emotionClick(e) {
    this.addToContent(e.emoji.native);
  }

  onChangeCheckboxCoupon(e) {
    if (this.isCouponControl.value !== true) {
      this.couponProgramIdControl.setValue(null);
    }
  }

  onSelectChangeChannel(e) {
    if (e) {
      if (e.type === 'facebook') {
        this.textareaLength = 640;
      } else if (e.type === 'zalo') {
        this.textareaLength = 2000;
      }
    }
  }

  quickCreateMessageTemplateModal() {
    const modalRef = this.modalService.open(TcareMessageTemplateCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạo mẫu tin';
    modalRef.result.then((val) => {
      debugger;
      this.loadMessageTemplate();
      this.onMessageTemplateSelect(val);
    });
  }

  quickUpdateMessageTemplateModal() {
    const modalRef = this.modalService.open(TcareMessageTemplateCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Sửa mẫu tin';
    modalRef.componentInstance.id = this.mau.id;
    modalRef.result.then((val) => {
      this.loadMessageTemplate();
      this.onMessageTemplateSelect(val);
    });
  }
}
