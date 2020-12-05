import { Component, ComponentFactoryResolver, ComponentRef, OnInit, QueryList, ViewChild, ViewChildren } from "@angular/core";
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
import { forkJoin } from 'rxjs';
import { AnchorHostDirective } from 'src/app/shared/anchor-host.directive';
import { SaleOrdersDotkhamCuComponent } from '../sale-orders-dotkham-cu/sale-orders-dotkham-cu.component';
import { FormBuilder } from '@angular/forms';

@Component({
  selector: "app-treatment-process-service-list",
  templateUrl: "./treatment-process-service-list.component.html",
  styleUrls: ["./treatment-process-service-list.component.css"],
})
export class TreatmentProcessServiceListComponent implements OnInit {
  saleOrderId: string;
  services: any;
  dotkhams: any[] = [];
  activeDotkham: any;
  // @ViewChild('dotkhamVC', {static: false}) dotkhamVC: any;
  @ViewChildren("dotkhamVC") domReference: QueryList<any>;

  @ViewChild(AnchorHostDirective, { static: true }) anchorHost: AnchorHostDirective;
  activeDotKhamRef: ComponentRef<SaleOrdersDotkhamCuComponent>;

  constructor(
    private route: ActivatedRoute,
    private dotKhamStepsOdataService: DotKhamStepsOdataService,
    private dotkhamOdataService: DotkhamOdataService,
    private saleOrdersOdataService: SaleOrdersOdataService,
    private modalService: NgbModal,
    private authService: AuthService,
    private notificationService: NotificationService,
    private errorService: AppSharedShowErrorService,
    private componentFactoryResolver: ComponentFactoryResolver,
    private fb: FormBuilder
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
      (result) => {
        if (step.IsDone) {
          this.notify('success', 'Đã hoàn thành tiến trình: ' + step.Name);
        }
      },
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
    if (!this.activeDotKhamRef) {
      this.notify('error', 'Vui lòng chỉnh sửa 1 đợt khám để thêm công đoạn thực hiện');
      return;
    }
    
    this.activeDotKhamRef.instance.linesFA.push(this.fb.group({
      NameStep: step.Name,
      ProductId: service.ProductId,
      Product: {
        Id: service.ProductId,
        Name: service.Name
      },
      Note: null,
      Teeth: this.fb.array([]),
      SaleOrderLineId: service.Id,
    }));
  }

  loadDotKhamList() {
    if (!this.saleOrderId) {
      return;
    }

    this.saleOrdersOdataService.getDotKhamListIds(this.saleOrderId).subscribe((res: any) => {
      const obs = res.value.map(id => {
        return this.dotkhamOdataService.getInfo(id);
      });

      forkJoin(obs).subscribe((result: any) => {
        this.dotkhams = result;
        console.log(this.dotkhams);

        this.dotkhams.forEach(dotkham => {
          const componentFactory = this.componentFactoryResolver.resolveComponentFactory(SaleOrdersDotkhamCuComponent);
          const viewContainerRef = this.anchorHost.viewContainerRef;
      
          const componentRef = viewContainerRef.createComponent<SaleOrdersDotkhamCuComponent>(componentFactory);
          componentRef.instance.dotkham = dotkham;

          componentRef.instance.btnSaveEvent.subscribe((dkVal: any) => {
            this.dotkhamOdataService.update(dotkham.Id, dkVal).subscribe(() => {
              this.notify('success', 'Lưu thành công');
              componentRef.instance.setEditModeActive(false);
              this.activeDotKhamRef = null;
            });
          });

          componentRef.instance.btnCancelEvent.subscribe(() => {
            this.dotkhamOdataService.getInfo(dotkham.Id).subscribe((result: any) => {
              componentRef.instance.setEditModeActive(false);
              dotkham = result;
              this.activeDotKhamRef = null;
            });
          });

          componentRef.instance.btnEditEvent.subscribe(() => {
            if (this.activeDotKhamRef) {
              this.notify('error', 'Vui lòng hoàn thành đợt khám đang chỉnh sửa');
            } else {
              this.activeDotkham = dotkham;
              this.activeDotKhamRef = componentRef;
              componentRef.instance.setEditModeActive(true);
            }
          });
        });
      });
    });

    // const state = {
    //   take: 10,
    //   filter: {
    //     logic: 'SaleOrderId',
    //     filters: [
    //       { field: 'SaleOrderId', operator: 'eq', value: this.saleOrderId }
    //     ]
    //   }
    // };
    // const options = {
    //   // expand: 'Lines'
    //   orderby: 'DateCreated desc'
    // };
    // this.dotkhamOdataService.fetch2(state, options).subscribe((res: any) => {
    //   this.dotkhams = res.data;
    // });
  }

  onCreateDotKham() {
    if (this.activeDotKhamRef) {
      this.notify('error', 'Vui lòng hoàn thành đợt khám đang chỉnh sửa')
      return false;
    }

    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(SaleOrdersDotkhamCuComponent);

    const viewContainerRef = this.anchorHost.viewContainerRef;

    const componentRef = viewContainerRef.createComponent<SaleOrdersDotkhamCuComponent>(componentFactory, 0);

    let dotkham = new DotKhamVm();
    dotkham.Date = new Date();
    dotkham.Sequence = this.dotkhams.length + 1;
    componentRef.instance.dotkham = dotkham;
    this.dotkhams.push(dotkham);

    componentRef.instance.setEditModeActive(true);
    this.activeDotKhamRef = componentRef;

    componentRef.instance.btnSaveEvent.subscribe((dkVal: any) => {
      if (!dotkham.Id) {
        this.saleOrdersOdataService.createDotkham(this.saleOrderId, dkVal).subscribe((res: any) => {
          this.notify('success', 'Lưu thành công');
          componentRef.instance.setEditModeActive(false);
          this.activeDotKhamRef = null;
          dotkham.Id = res.Id;
        });
      } else {
        this.dotkhamOdataService.update(dotkham.Id, dkVal).subscribe((res: any) => {
          this.notify('success', 'Lưu thành công');
          componentRef.instance.setEditModeActive(false);
          this.activeDotKhamRef = null;
        });
      }
    });

    componentRef.instance.btnEditEvent.subscribe(() => {
      if (this.activeDotKhamRef) {
        this.notify('error', 'Vui lòng hoàn thành đợt khám đang chỉnh sửa');
      } else {
        this.activeDotkham = dotkham;
        this.activeDotKhamRef = componentRef;
        componentRef.instance.setEditModeActive(true);
      }
    });

    componentRef.instance.btnCancelEvent.subscribe(() => {
      if (!dotkham.Id) {
        viewContainerRef.remove(0);
        this.activeDotKhamRef = null;
        this.dotkhams.splice(0, 1);
      } else {
        this.dotkhamOdataService.getInfo(dotkham.Id).subscribe((result: any) => {
          componentRef.instance.setEditModeActive(false);
          dotkham = result;
          this.activeDotKhamRef = null;
        });
      }
    
    });

    this.activeDotkham = dotkham;

    // if (this.activeDotkham) {
    //   this.notify('error', 'Vui lòng hoàn tất đợt khám đang thao tác');
    //   return;
    // }
    // const dotkham = new DotKhamVm();
    // dotkham.Date = new Date();
    // dotkham.Sequence = this.dotkhams.length + 1;
    // dotkham.SaleOrderId = this.saleOrderId;
    // dotkham.CompanyId = this.authService.userInfo.companyId;
    // this.dotkhams.unshift(dotkham);
    // this.activeDotkham = dotkham;
  }

  onDkSaveEvent(val, dotkham) {
    val.SaleOrderId = this.saleOrderId;
    if (!val.Id) {
      delete val['Id'];
      this.dotkhamOdataService.create(val).subscribe((res: any) => {
        this.notify('success', 'Lưu thành công');
        if (!dotkham.Id) {
          dotkham.Id = res.Id;
        }
        console.log(dotkham);
      });
    } else {
      this.dotkhamOdataService.update(val.Id, val).subscribe((res: any) => {
        this.notify('success', 'Lưu thành công');
        console.log(dotkham);
      });
    }
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
