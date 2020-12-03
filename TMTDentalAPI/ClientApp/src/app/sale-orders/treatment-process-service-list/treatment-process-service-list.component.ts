import { Component, OnInit, QueryList, ViewChild, ViewChildren } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { DotKhamStepsOdataService } from 'src/app/shared/services/dot-kham-stepsOdata.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DotKhamCreateUpdateDialogComponent } from 'src/app/shared/dot-kham-create-update-dialog/dot-kham-create-update-dialog.component';
import { DotKhamLineDisplay, DotkhamOdataService, DotKhamVm } from 'src/app/shared/services/dotkham-odata.service';
import { SaleOrdersOdataService } from "src/app/shared/services/sale-ordersOdata.service";
import { SaleOrderCreateDotKhamDialogComponent } from '../sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { TreatmentProcessServiceDialogComponent } from '../treatment-process-service-dialog/treatment-process-service-dialog.component';
import { AuthService } from 'src/app/auth/auth.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

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
  // @ViewChild('dotkhamVC', {static: false}) dotkhamVC: any;
  @ViewChildren("dotkhamVC") domReference: QueryList<any>;

  constructor(
    private route: ActivatedRoute,
    private dotKhamStepsOdataService: DotKhamStepsOdataService,
    private dotkhamOdataService: DotkhamOdataService,
    private saleOrdersOdataService: SaleOrdersOdataService,
    private modalService: NgbModal,
    private authService: AuthService,
    private notificationService: NotificationService, 
    private errorService: AppSharedShowErrorService
  ) { }

  ngOnInit() {
    this.saleOrderId = this.route.queryParams['value'].id;

    this.loadServiceList();
    this.loadDotKhamList();
  }

  loadServiceList() {
    if (this.saleOrderId) {
      this.saleOrdersOdataService.getDotKhamStepByOrderLine(this.saleOrderId).subscribe(
        (result) => {
          this.services = result['value'];
        },
        (error) => { 
          this.errorService.show(error);
        }
      );
    }
  }

  formatTeethList(teethList) {
    return teethList.map(x => x.Name).join(', ');
  }

  checkStatusDotKhamStep(step) {
    step.IsDone = !step.IsDone;
    var value = {
      Id: step.Id,
      IsDone: step.IsDone
    }
    this.dotKhamStepsOdataService.patch(step.Id, value).subscribe(
      (result) => { },
      (error) => {
        this.errorService.show(error);
      }
    );

    this.loadDotKhamList();
  }

  editTreatmentProcess(service) {
    let modalRef = this.modalService.open(TreatmentProcessServiceDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.service = service;

    modalRef.result.then((result) => {
      this.notify('success', 'Đã lưu tiến trình điều trị');
      this.loadServiceList();
    }, 
    (error) => { }
    );
  }

  sendDotKhamStep(service, step) {
    if (!this.activeDotkham) {
      this.notify('error', 'Không có đợt khám để thêm công đoạn điều trị');
      return;
    }
    debugger;
    const line = new DotKhamLineDisplay();
    line.Name = step.Name;
    line.DotKhamId = this.activeDotkham.Id;
    line.ProductId = service.ProductId;
    line.Product = {
      Id: service.ProductId,
      Name: service.Name
    };
    line.State = 'draft';
    line.Sequence = this.activeDotkham.Lines.length + 1;
    this.activeDotkham.Lines.push(line);
  }

  loadDotKhamList() {
    if (!this.saleOrderId) {
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
      // expand: 'Lines'
      orderby: 'DateCreated desc'
    };
    this.dotkhamOdataService.fetch2(state, options).subscribe((res: any) => {
      this.dotkhams = res.data;
    });
  }

  onCreateDotKham() {
    if (this.activeDotkham) {
      this.notify('error', 'Vui lòng hoàn tất đợt khám đang thao tác');
      return;
    }
    const dotkham = new DotKhamVm();
    dotkham.Date = new Date();
    dotkham.Name = 'Đợt khám ' + (this.dotkhams.length + 1);
    dotkham.SaleOrderId = this.saleOrderId;
    dotkham.CompanyId = this.authService.userInfo.companyId;
    this.dotkhams.unshift(dotkham);
    this.activeDotkham = dotkham;
  }

  dotkhamChange(e, i) {
    if (e.dotkham) {
      this.dotkhams[i] = e.dotkham;
    }
    if (e.isDelete) {
      this.dotkhams.splice(i, 1);
    }
    this.activeDotkham = e.activeDotkham;
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }
}
