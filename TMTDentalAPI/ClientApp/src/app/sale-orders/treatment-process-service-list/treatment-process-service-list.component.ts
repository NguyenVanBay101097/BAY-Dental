import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { DotKhamStepsOdataService } from 'src/app/shared/services/dot-kham-stepsOdata.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DotKhamCreateUpdateDialogComponent } from 'src/app/shared/dot-kham-create-update-dialog/dot-kham-create-update-dialog.component';
import { DotKhamLineDisplay, DotkhamOdataService } from 'src/app/shared/services/dotkham-odata.service';
import { SaleOrdersOdataService } from "src/app/shared/services/sale-ordersOdata.service";
import { SaleOrderCreateDotKhamDialogComponent } from '../sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { TreatmentProcessServiceDialogComponent } from '../treatment-process-service-dialog/treatment-process-service-dialog.component';

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
    private dotKhamStepsOdataService: DotKhamStepsOdataService,
    private dotkhamOdataService: DotkhamOdataService,
    private saleOrderOdataService: SaleOrdersOdataService,
    private modalService: NgbModal
  ) {}

  ngOnInit() {
    this.saleOrderId = this.route.queryParams['value'].id;

    this.loadDotKhamList();
    if (this.saleOrderId) {
      this.saleOrderOdataService.getDotKhamStepByOrderLine(this.saleOrderId).subscribe(
        (result) => {
          console.log(result);
          this.services = result['value'];
        },
        (error) => { }
      );
    }
  }

  checkStatusDotKhamStep(step) {
    step.IsDone= !step.IsDone;
    var value = {
      Id: step.Id,
      IsDone: step.IsDone
    }
    this.dotKhamStepsOdataService.patch(step.Id, value).subscribe(
      (result) => {
        console.log(result);
      },
      (error) => {}
    );

    this.loadDotKhamList();
  }

  editTreatmentProcess(service) {
    let modalRef = this.modalService.open(TreatmentProcessServiceDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.service = service;

    modalRef.result.then((result) => {

    }, 
    (error) => {}
    );
  }

  sendDotKhamStep(service, step) {
    const line = new DotKhamLineDisplay();
    console.log(service);
    console.log(step);
    line.Name = step.Name;
    line.DotKhamId = this.activeDotkham.Id;
    line.ProductId = service.Id;
    line.Product = service.Name;
    line.State = 'draft';
    line.Sequence = this.activeDotkham.Lines.length + 1;
    this.activeDotkham.Lines.push(line);
    console.log(line);
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
