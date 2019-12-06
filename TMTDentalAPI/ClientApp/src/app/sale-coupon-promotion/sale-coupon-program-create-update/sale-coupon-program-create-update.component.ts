import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductPriceListBasic, ProductPricelistPaged } from 'src/app/price-list/price-list';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { PriceListService } from 'src/app/price-list/price-list.service';
import * as _ from 'lodash';
import { SaleCouponProgramDisplay, SaleCouponProgramService } from '../sale-coupon-program.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleCouponProgramGenerateCouponsDialogComponent } from '../sale-coupon-program-generate-coupons-dialog/sale-coupon-program-generate-coupons-dialog.component';

@Component({
  selector: 'app-sale-coupon-program-create-update',
  templateUrl: './sale-coupon-program-create-update.component.html',
  styleUrls: ['./sale-coupon-program-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleCouponProgramCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  program: SaleCouponProgramDisplay = new SaleCouponProgramDisplay();

  filteredPricelists: ProductPriceListBasic[];
  @ViewChild('pricelistCbx', { static: true }) pricelistCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private programService: SaleCouponProgramService,
    private router: Router, private route: ActivatedRoute, private notificationService: NotificationService,
    private modalService: NgbModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      ruleMinimumAmount: 0,
      discountType: 'percentage',
      discountPercentage: 0,
      discountFixedAmount: 0,
      validityDuration: 0,
      programType: null
    });

    this.route.queryParamMap.subscribe(params => {
      this.id = params.get("id");
      if (this.id) {
        this.loadRecord();
      } else {
        this.formGroup = this.fb.group({
          name: [null, Validators.required],
          ruleMinimumAmount: 0,
          discountType: 'percentage',
          discountPercentage: 0,
          discountFixedAmount: 0,
          validityDuration: 0,
          programType: null
        });

        this.program = new SaleCouponProgramDisplay();
      }
    });

    // this.id = this.route.snapshot.paramMap.get('id');
    // if (this.id) {
    //   this.loadRecord();
    // }

    // this.pricelistCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.pricelistCbx.loading = true)),
    //   switchMap(value => this.searchPricelists(value))
    // ).subscribe(result => {
    //   this.filteredPricelists = result.items;
    //   this.pricelistCbx.loading = false;
    // });

    // this.loadPricelists();
  }

  viewCoupons() {
    this.router.navigate(['/coupons'], { queryParams: { program_id: this.id } });
  }

  get discountType() {
    return this.formGroup.get('discountType').value;
  }

  generateCoupons() {
    if (this.id) {
      let modalRef = this.modalService.open(SaleCouponProgramGenerateCouponsDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.programId = this.id;
      modalRef.result.then(() => {
        this.loadRecord();
      }, () => {
      });
    } else {
      if (!this.formGroup.valid) {
        return false;
      }

      var value = this.formGroup.value;
      this.programService.create(value).subscribe(result => {
        this.router.navigate(['/coupon-programs/form'], { queryParams: { id: result.id } });
        let modalRef = this.modalService.open(SaleCouponProgramGenerateCouponsDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.programId = result.id;
        modalRef.result.then(() => {
          this.loadRecord();
        }, () => {
        });
      });
    }

  }


  loadRecord() {
    this.programService.get(this.id).subscribe(result => {
      this.program = result;
      this.formGroup.patchValue(result);
    });
  }

  createNew() {
    this.router.navigate(['/coupon-programs/form']);
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    if (this.id) {
      this.programService.update(this.id, value).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadRecord();
      });
    } else {
      this.programService.create(value).subscribe(result => {
        this.router.navigate(['/coupon-programs/form'], { queryParams: { id: result.id } });
      });
    }
  }

}

