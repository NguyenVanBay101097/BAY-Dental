import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { StockPickingMlDialogComponent } from '../stock-picking-ml-dialog/stock-picking-ml-dialog.component';
import { StockMoveDisplay, StockPickingService, StockPickingDefaultGet } from '../stock-picking.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { PartnerFilter, PartnerService } from 'src/app/partners/partner.service';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { StockPickingTypeService, StockPickingTypeBasic } from 'src/app/stock-picking-types/stock-picking-type.service';
import { StockMoveService, StockMoveOnChangeProduct } from 'src/app/stock-moves/stock-move.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductPaged } from 'src/app/products/product.service';
import { debug } from 'util';
declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-stock-picking-create-update',
  templateUrl: './stock-picking-create-update.component.html',
  styleUrls: ['./stock-picking-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class StockPickingCreateUpdateComponent implements OnInit {

  pickingForm: FormGroup;
  pickingTypeId: string;
  pickingType: StockPickingTypeBasic;
  opened = false;
  filteredPartners: PartnerSimple[] = [];
  id: string;
  productPaged: ProductPaged;

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private route: ActivatedRoute,
    private windowService: WindowService, private intlService: IntlService,
    private stockPickingService: StockPickingService, private router: Router, private partnerService: PartnerService,
    private notificationService: NotificationService, private pickingTypeService: StockPickingTypeService,
    private stockMoveService: StockMoveService) { }

  ngOnInit() {
    this.pickingForm = this.fb.group({
      partner: null,
      dateObj: new Date(),
      note: null,
      moveLines: this.fb.array([]),
      companyId: null,
      locationId: null,
      locationDestId: null,
      pickingTypeId: null,
      state: null,
      name: null
    });

    this.productPaged = new ProductPaged();
    this.productPaged.limit = 10;
    this.productPaged.type = "product";

    setTimeout(() => {
      this.partnerCbx.focus();
    }, 200);

    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerCbx.loading = true)),
      switchMap(value => this.searchPartners(value))
    ).subscribe(result => {
      this.filteredPartners = result;
      this.partnerCbx.loading = false;
    });

    this.id = this.route.snapshot.paramMap.get('id');
    this.pickingTypeId = this.route.snapshot.queryParamMap.get('picking_type_id');
    if (this.id) {
      this.loadRecord();
    } else {
      this.loadDefault();
    }

    this.loadPickingType();

  }

  loadFilteredPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = result;
    });
  }

  searchPartners(search?: string) {
    var val = new PartnerPaged();
    val.search = search;

    if (this.pickingType && this.pickingType.code === 'incoming') {
      val.supplier = true;
    } else {
      val.customer = true;
    }
    return this.partnerService.getAutocompleteSimple(val);
  }

  loadPickingType() {
    if (this.pickingTypeId) {
      this.pickingTypeService.getBasic(this.pickingTypeId).subscribe(result => {
        this.pickingType = result;
        this.loadFilteredPartners();
      });
    }
  }

  loadDefault() {
    var val = new StockPickingDefaultGet();
    val.defaultPickingTypeId = this.pickingTypeId;
    this.stockPickingService.defaultGet(val).subscribe(result => {
      this.pickingForm.patchValue(result);
    });
  }

  loadRecord() {
    this.stockPickingService.get(this.id).subscribe(result => {
      if (result.partner) {
        this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
      }
      this.pickingForm.patchValue(result);
      this.moveLines.clear();
      result.moveLines.forEach(line => {
        this.moveLines.push(this.fb.group(line));
      });
    });
  }

  onProductSelected(value) {
    var item = new StockMoveDisplay();
    item.product = new ProductSimple();
    item.product.id = value.id;
    item.productId = value.id;
    item.product.name = value.name;
    item.name = value.name;
    item.productUOMQty = 1;
    let flag = true;
    this.moveLines.controls.forEach(line => {
      if (line.get('productId').value == item.productId) {
        line.get('productUOMQty').setValue(line.get('productUOMQty').value + 1);
        flag = false;
        return;
      }
    })

    if (flag) {
      this.moveLines.push(this.fb.group(item));
      this.focusLastRow();
    }
  }

  focusLastRow() {
    setTimeout(() => {
      var $lastTr = $('tr:last', $('#table_details tbody'));
      $('input:first', $lastTr).focus();
    }, 70);
  }

  showAddLineModal() {
    const windowRef = this.windowService.open({
      title: 'Thêm chi tiết',
      content: StockPickingMlDialogComponent,
      resizable: false,
    });

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (result instanceof WindowCloseResult) {
      } else {
        var line = result as StockMoveDisplay;
        var lines = this.pickingForm.get('moveLines') as FormArray;
        lines.push(this.fb.group(line));
      }
    });
  }

  editLine(line: FormGroup) {
    const windowRef = this.windowService.open({
      title: 'Sửa chi tiết',
      content: StockPickingMlDialogComponent,
      resizable: false,
    });

    const instance = windowRef.content.instance;
    instance.line = line.value;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (result instanceof WindowCloseResult) {
      } else {
        var a = result as StockMoveDisplay;
        line.patchValue(result);
      }
    });
  }

  deleteLine(index: number) {
    this.moveLines.removeAt(index);
  }

  get pickingState() {
    return this.pickingForm.get('state').value;
  }

  get pickingName() {
    return this.pickingForm.get('name').value;
  }

  get moveLines() {
    return this.pickingForm.get('moveLines') as FormArray;
  }

  onSaveOrUpdate() {
    if (!this.pickingForm.valid) {
      return;
    }

    var val = this.pickingForm.value;
    val.partnerId = val.partner ? val.partner.id : null;
    val.date = this.intlService.formatDate(val.dateObj, 'g', 'en-US');
    if (this.id) {
      this.stockPickingService.update(this.id, val).subscribe(() => {
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
      this.stockPickingService.create(val).subscribe(result => {
        this.router.navigate(['pickings/edit/', result.id], { queryParams: { picking_type_id: this.pickingTypeId } });
      });
    }
  }

  actionDone() {
    if (this.id) {
      if (this.pickingForm.dirty) {
        //update and done
        if (!this.pickingForm.valid) {
          return;
        }

        var val = this.pickingForm.value;
        val.partnerId = val.partner ? val.partner.id : null;
        val.date = this.intlService.formatDate(val.dateObj, 'g', 'en-US');

        this.stockPickingService.update(this.id, val).subscribe(() => {
          console.log('updated');
          this.stockPickingService.actionDone([this.id]).subscribe(() => {
            console.log('done');
            this.loadRecord();
          });
        });
      } else {
        //only need done
        this.stockPickingService.actionDone([this.id]).subscribe(() => {
          console.log('done');
          this.loadRecord();
        });
      }
    } else {
      //save and done
      if (!this.pickingForm.valid) {
        return;
      }

      var val = this.pickingForm.value;
      val.partnerId = val.partner ? val.partner.id : null;
      val.date = this.intlService.formatDate(val.dateObj, 'g', 'en-US');

      this.stockPickingService.create(val).subscribe(result => {
        console.log('created');
        this.stockPickingService.actionDone([result.id]).subscribe(() => {
          console.log('done');
          this.router.navigate(['pickings/edit/', result.id], { queryParams: { picking_type_id: this.pickingTypeId } });
        }, () => {
          this.router.navigate(['pickings/edit/', result.id], { queryParams: { picking_type_id: this.pickingTypeId } });
        });
      });
    }
  }
}
