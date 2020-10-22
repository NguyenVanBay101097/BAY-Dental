import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NotificationService } from '@progress/kendo-angular-notification';
import { validate } from 'fast-json-patch';
import { stringify } from 'querystring';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SaleCouponProgramPaged, SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
import { TCareMessageTemplatePaged, TCareMessageTemplateService } from '../tcare-message-template.service';
import { TcareMessageTemplateContentComponent } from './tcare-message-template-content/tcare-message-template-content.component';

@Component({
  selector: 'app-tcare-message-template-cu-dialog',
  templateUrl: './tcare-message-template-cu-dialog.component.html',
  styleUrls: ['./tcare-message-template-cu-dialog.component.css']
})
export class TcareMessageTemplateCuDialogComponent implements OnInit {
  id: string;
  title: string;
  formGroup: FormGroup;
  listCoupon: any;
  templates: any[] = [
    {
      text: null,
      templateType: 'text'
    }
  ];

  @ViewChild('textTemp', {static: false}) textTemp: TcareMessageTemplateContentComponent;
  // @ViewChild('couponCbx', { static: true }) couponCbx: ComboBoxComponent;


  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private templateService: TCareMessageTemplateService,
    private notificationService: NotificationService,
    private saleCouponService: SaleCouponProgramService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      content: [this.templates, Validators.required],
      type: ['facebook', Validators.required],
      couponProgram: null
    });

    setTimeout(() => {
      if (this.id) {
        this.loadDataFromApi();
      }
    });

    // this.couponCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.couponCbx.loading = true)),
    //   switchMap(value => this.searchCoupon(value))
    // ).subscribe((result: any) => {
    //   this.listCoupon = result.items;
    //   this.couponCbx.loading = false;
    // });

    // this.loadCoupon();

  }

  get contentControl() { return this.formGroup.get('content'); }
  get typeControl() { return this.formGroup.get('type'); }

  searchCoupon(q?: string) {
    const val = new SaleCouponProgramPaged();
    val.search = q || '';
    val.programType = 'coupon_program';
    return this.saleCouponService.getPaged(val);
  }

  loadCoupon() {
    this.searchCoupon().subscribe((result: any) => {
      this.listCoupon = result.items;
    });
  }

  loadDataFromApi() {
    this.templateService.get(this.id).subscribe((res: any) => {
      this.formGroup.patchValue(res);
      this.templates = JSON.parse(res.content);
      this.contentControl.setValue(this.templates);
    });
  }

  notify(title, isSuccess = true) {
    this.notificationService.show({
      content: title,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: isSuccess ? 'success' : 'error', icon: true },
    });
  }

  onSave() {
    this.updateValue();

    if (this.formGroup.invalid) { return false; }
    const val = this.formGroup.value;
    val.content = JSON.stringify(this.templates);
    // val.couponProgramId = val.couponProgram ? val.couponProgram.id : null;

    if (this.id) {
      this.templateService.update(this.id, val).subscribe(
        (res) => {
          this.notify('thành công', true);
          val.id = this.id;
          this.activeModal.close(val);
        }
      );
    } else {
      this.templateService.create(val).subscribe((res) => {
        this.notify('thành công', true);
        this.activeModal.close(res);
      });
    }
  }

  updateValue() {
    if (this.textTemp) {
      const val = this.textTemp.onSave();
      this.templates[val.index] = val.template;
    }
  }

  getTextareaLength() {
    if (this.typeControl.value === 'facebook') {
      return 640;
    } else
    if (this.typeControl.value === 'zalo') {
      return 2000;
    }
  }

}
