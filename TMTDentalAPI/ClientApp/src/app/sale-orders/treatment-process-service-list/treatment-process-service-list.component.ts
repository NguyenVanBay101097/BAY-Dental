import { Component, ComponentFactoryResolver, ComponentRef, Input, OnInit, QueryList, ViewChild, ViewChildren } from "@angular/core";
import { FormBuilder } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { forkJoin } from 'rxjs';
import { SaleOrderService } from "src/app/core/services/sale-order.service";
import { DotKhamStepService } from "src/app/dot-khams/dot-kham-step.service";
import { DotKhamService } from "src/app/dot-khams/dot-kham.service";
import { AnchorHostDirective } from 'src/app/shared/anchor-host.directive';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { SaleOrdersDotkhamCuComponent } from '../sale-orders-dotkham-cu/sale-orders-dotkham-cu.component';
import { TreatmentProcessServiceDialogComponent } from '../treatment-process-service-dialog/treatment-process-service-dialog.component';

@Component({
  selector: "app-treatment-process-service-list",
  templateUrl: "./treatment-process-service-list.component.html",
  styleUrls: ["./treatment-process-service-list.component.css"],
})
export class TreatmentProcessServiceListComponent implements OnInit {
  @Input() saleOrderId: string;
  services: any;
  dotkhams: any[] = [];
  activeDotkham: any;
  toothType = [
    { name: "Hàm trên", value: "upper_jaw" },
    { name: "Nguyên hàm", value: "whole_jaw" },
    { name: "Hàm dưới", value: "lower_jaw" },
  ];
  // @ViewChild('dotkhamVC', {static: false}) dotkhamVC: any;
  @ViewChildren("dotkhamVC") domReference: QueryList<any>;

  @ViewChild(AnchorHostDirective, { static: true }) anchorHost: AnchorHostDirective;
  activeDotKhamRef: ComponentRef<SaleOrdersDotkhamCuComponent>;

  constructor(
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private errorService: AppSharedShowErrorService,
    private componentFactoryResolver: ComponentFactoryResolver,
    private fb: FormBuilder,
    private saleOrderService: SaleOrderService,
    private dotKhamService: DotKhamService,
    private dotKhamStepService: DotKhamStepService
  ) { }

  ngOnInit() {
    // this.saleOrderId = this.route.queryParams['value'].id;
    // console.log('SaleOrderId: ', this.saleOrderId);
    // let orderId = this.route.snapshot.paramMap.get('id');

    this.loadServiceList();
    this.loadDotKhamList();
  }

  loadServiceList() {
    if (this.saleOrderId) {
      this.saleOrderService.getDotKhamStepByOrderLine(this.saleOrderId).subscribe(
        (result) => {
          this.services = result;
        },
        (error) => {
        }
      );
    }
  }

  formatTeethList(service) {
    let teethList = '';
    if (service.toothType && service.toothType != 'manual') {
      teethList = this.toothType.find(x => x.value == service.toothType).name;
    }
    else {
      teethList = service.teeth.map(x => x.name).join(', ');
    }
    return teethList;
  }

  checkStatusDotKhamStep(step) {
    var value = {
      id: step.id,
      isDone: !step.isDone
    }
    this.dotKhamStepService.updateIsDone(value).subscribe(
      (result) => {
        this.notify('success', 'Cập nhật thành công');
        step.isDone = !step.isDone;
      },
      (error) => {
        this.errorService.show(error);
      }
    );
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
      this.notify('error', 'Vui lòng tạo hoặc chỉnh sửa 1 đợt khám để thêm công đoạn thực hiện');
      return;
    }

    this.activeDotKhamRef.instance.linesFA.push(this.fb.group({
      nameStep: step.name,
      productId: service.productId,
      product: {
        id: service.productId,
        name: service.name
      },
      note: null,
      teeth: this.fb.array([]),
      saleOrderLineId: service.id,
    }));
  }

  loadDotKhamList() {
    if (!this.saleOrderId) {
      return;
    }

    this.saleOrderService.getDotKhamListIds(this.saleOrderId).subscribe((res: any) => {
      const obs = res.map(id => {
        return this.dotKhamService.getInfo(id);
      });

      forkJoin(obs).subscribe((result: any) => {
        this.dotkhams = result;
        this.renderDotKhamList();
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

  renderDotKhamList() {
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(SaleOrdersDotkhamCuComponent);
    const viewContainerRef = this.anchorHost.viewContainerRef;
    viewContainerRef.clear();

    this.dotkhams.forEach(dotkham => {
      var index = this.dotkhams.indexOf(dotkham);
      var sequence = this.dotkhams.length - index;

      const componentRef = viewContainerRef.createComponent<SaleOrdersDotkhamCuComponent>(componentFactory);
      componentRef.instance.dotkham = dotkham;
      componentRef.instance.index = index;
      componentRef.instance.sequence = sequence;

      componentRef.instance.btnSaveEvent.subscribe((dkVal: any) => {
        this.dotKhamService.update(dotkham.id, dkVal).subscribe(() => {
          this.notify('success', 'Lưu thành công');

          this.dotKhamService.getInfo(dotkham.id).subscribe((result: any) => {
            componentRef.instance.setEditModeActive(false);
            dotkham = result;
            componentRef.instance.dotkham = result;
            componentRef.instance.loadRecord();
            this.activeDotKhamRef = null;
          });
        });
      });

      componentRef.instance.btnCancelEvent.subscribe(() => {
        this.dotKhamService.getInfo(dotkham.id).subscribe((result: any) => {
          componentRef.instance.setEditModeActive(false);
          dotkham = result;
          componentRef.instance.dotkham = result;
          componentRef.instance.loadRecord();
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

      componentRef.instance.btnDeleteEvent.subscribe((dk) => {
        var index = this.dotkhams.indexOf(dk);
        this.dotkhams.splice(index, 1);
        this.renderDotKhamList();
      });
    });
  }

  onCreateDotKham() {
    if (this.activeDotKhamRef) {
      this.notify('error', 'Vui lòng hoàn thành đợt khám đang chỉnh sửa')
      return false;
    }

    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(SaleOrdersDotkhamCuComponent);

    const viewContainerRef = this.anchorHost.viewContainerRef;

    const componentRef = viewContainerRef.createComponent<SaleOrdersDotkhamCuComponent>(componentFactory, 0);

    let dotkham = <any>{};
    dotkham.date = new Date();
    dotkham.lines = [];
    dotkham.irAttachments = [];
    this.dotkhams.unshift(dotkham);

    var index = this.dotkhams.indexOf(dotkham);
    var sequence = this.dotkhams.length - index;
    componentRef.instance.dotkham = dotkham;
    componentRef.instance.index = index;
    componentRef.instance.sequence = sequence;
    componentRef.instance.setEditModeActive(true);
    this.activeDotKhamRef = componentRef;

    componentRef.instance.btnSaveEvent.subscribe((dkVal: any) => {
      if (!dotkham.id) {
        this.saleOrderService.createDotkham(this.saleOrderId, dkVal).subscribe((res: any) => {
          this.notify('success', 'Lưu thành công');
          dotkham.id = res.id;
          dotkham.name = res.name;
          componentRef.instance.setEditModeActive(false);
          this.activeDotKhamRef = null;
        });
      } else {
        this.dotKhamService.update(dotkham.id, dkVal).subscribe((res: any) => {
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

    componentRef.instance.btnDeleteEvent.subscribe((dk) => {
      var index = this.dotkhams.indexOf(dk);
      this.dotkhams.splice(index, 1);
      viewContainerRef.remove(index);
      this.activeDotKhamRef = null;
    });

    componentRef.instance.btnCancelEvent.subscribe(() => {
      if (!dotkham.id) {
        viewContainerRef.remove(0);
        this.activeDotKhamRef = null;
        this.dotkhams.splice(0, 1);
      } else {
        this.dotKhamService.getInfo(dotkham.id).subscribe((result: any) => {
          componentRef.instance.setEditModeActive(false);
          dotkham = result;
          componentRef.instance.dotkham = result;
          componentRef.instance.loadRecord();
          this.activeDotKhamRef = null;
        });
      }

    });

    this.activeDotkham = dotkham;
  }

  onDkSaveEvent(val, dotkham) {
    val.saleOrderId = this.saleOrderId;
    if (!val.id) {
      delete val['id'];
      this.dotKhamService.create(val).subscribe((res: any) => {
        this.notify('success', 'Lưu thành công');
        if (!dotkham.Id) {
          dotkham.id = res.Id;
        }
        console.log(dotkham);
      });
    } else {
      this.dotKhamService.update(val.id, val).subscribe((res: any) => {
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
