import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleOrderDisplay } from '../sale-order-display';
import { SaleOrderService } from '../../core/services/sale-order.service';
import { SaleOrderCuFormComponent } from '../sale-order-cu-form/sale-order-cu-form.component';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-sale-order-cu-dialog',
  templateUrl: './sale-order-cu-dialog.component.html',
  styleUrls: ['./sale-order-cu-dialog.component.css']
})
export class SaleOrderCuDialogComponent implements OnInit {
  title: string;
  id: string;
  saleOrder: SaleOrderDisplay;
  defaultVal: any;
  @ViewChild(SaleOrderCuFormComponent, {static: true}) formComponent: SaleOrderCuFormComponent;

  constructor(public activeModal: NgbActiveModal, private saleOrderService: SaleOrderService,
    private showErrorService: AppSharedShowErrorService) { }

  ngOnInit() {
    setTimeout(() => {
      if (this.id) {
        this.saleOrderService.get(this.id).subscribe((result) => {
          this.saleOrder = result;
        });
      } else {
        this.saleOrderService.defaultGet(this.defaultVal || {}).subscribe((result) => {
          this.saleOrder = result;
        });
      }
    });
  }

  onConfirm() {
    if (!this.formComponent.isFormValid) {
      return false;
    }

    var data = this.formComponent.getDataSave();
    if (this.id) {
      this.saleOrderService.update(this.id, data).subscribe(() => {
        this.saleOrderService.actionConfirm([this.id]).subscribe(() => {
          this.activeModal.close(null);
        }, (err) => {
          this.showErrorService.show(err);
        });
      }, (err) => {
        this.showErrorService.show(err);
      });
    } else {
      this.saleOrderService.create(data).subscribe((res) => {
        this.saleOrderService.actionConfirm([res.id]).subscribe(() => {
          this.activeModal.close(res);
        }, (err) => {
          this.showErrorService.show(err);
        });
      }, (err) => {
        this.showErrorService.show(err);
      });
    }
  }

  onSave() {
    if (!this.formComponent.isFormValid) {
      return false;
    }

    var data = this.formComponent.getDataSave();
    if (this.id) {
      this.saleOrderService.update(this.id, data).subscribe(() => {
        this.activeModal.close(null);
      }, (err) => {
        this.showErrorService.show(err);
      });
    } else {
      this.saleOrderService.create(data).subscribe((res) => {
        this.activeModal.close(res);
      }, (err) => {
        this.showErrorService.show(err);
      });
    }
  }
}
