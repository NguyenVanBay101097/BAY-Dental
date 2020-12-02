import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DotKhamCreateUpdateDialogComponent } from 'src/app/shared/dot-kham-create-update-dialog/dot-kham-create-update-dialog.component';
import { DotkhamOdataService } from 'src/app/shared/services/dotkham-odata.service';
import { SaleOrdersOdataService } from "src/app/shared/services/sale-ordersOdata.service";
import { SaleOrderCreateDotKhamDialogComponent } from '../sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';

@Component({
  selector: "app-treatment-process-service-list",
  templateUrl: "./treatment-process-service-list.component.html",
  styleUrls: ["./treatment-process-service-list.component.css"],
})
export class TreatmentProcessServiceListComponent implements OnInit {
  saleOrderId: string;
  services: any;
  dotkhams: any[];
  activeDotkham: any;

  constructor(
    private route: ActivatedRoute,
    private saleOrderOdataService: SaleOrdersOdataService,
    private dotkhamOdataService: DotkhamOdataService,
    private modalService: NgbModal
  ) {}

  ngOnInit() {
    this.saleOrderId = this.route.queryParams['value'].id;

    if (this.saleOrderId) {
      this.saleOrderOdataService
        .getDotKhamStepByOrderLine(this.saleOrderId)
        .subscribe(
          (result) => {
            console.log(result);
            this.services = result['value'];
          },
          (error) => {}
        );
    }

    this.loadDotKhamList();
  }

  loadDotKhamList() {
    if ( !this.saleOrderId) {
      return;
    }
    const state = {
      take: 10,
      filter: {
        logic: 'SaleOrderId',
        filters: [
          { field: 'SaleOrderId', operator: 'eq', value: this.saleOrderId }
        ]
      }
    };
    const options = {
      // expand: 'DotKhamImages'
    };
    this.dotkhamOdataService.fetch2(state, options).subscribe((res: any) => {
      this.dotkhams = res.data;
    });
  }

  onCreateDotKham() {
    // const dotkham = {

    // };
    // this.dotkhams.unshift(dotkham);
    if (this.saleOrderId) {
      let modalRef = this.modalService.open(SaleOrderCreateDotKhamDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Tạo đợt khám';
      modalRef.componentInstance.saleOrderId = this.saleOrderId;

      modalRef.result.then(res => {
        if (res.view) {
          this.actionEditDotKham(res.result);
          this.loadDotKhamList();
        } else {
          this.loadDotKhamList();
          // $('#myTab a[href="#profile"]').tab('show');
        }
      }, () => {
      });
    }
  }

  actionEditDotKham(item) {
    let modalRef = this.modalService.open(DotKhamCreateUpdateDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cập nhật đợt khám';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.partnerId = item.Partner.Id;
    if (item.Partner)
      modalRef.componentInstance.partner = item.Partner;
    modalRef.result.then(() => {
      this.loadDotKhamList();
    }, () => {
    });
  }
}
