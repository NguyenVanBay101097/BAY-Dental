import { PartnerFilter, PartnerInfoFilter } from 'src/app/partners/partner.service';
import { Component, Inject, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { GridComponent, GridDataResult, PageChangeEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { PartnerPaged, PartnerBasic } from '../partner-simple';
import { map, debounceTime, distinctUntilChanged, tap, switchMap, subscribeOn } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { NgbModal, NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PartnerImportComponent } from '../partner-import/partner-import.component';
import { PartnerCategoryBasic, PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnerActivePatch, PartnerInfoPaged, PartnerService } from '../partner.service';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { PartnerCategoryPopoverComponent } from './partner-category-popover/partner-category-popover.component';
import { PartnersBindingDirective } from 'src/app/shared/directives/partners-binding.directive';
import { PartnerCustomerAutoGenerateCodeDialogComponent } from '../partner-customer-auto-generate-code-dialog/partner-customer-auto-generate-code-dialog.component';
import { NotificationService } from '@progress/kendo-angular-notification';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { MemberLevelAutoCompleteReq, MemberLevelService } from 'src/app/member-level/member-level.service';
import { values } from 'lodash';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { CardCardService } from 'src/app/card-cards/card-card.service';
import { CardTypeService } from 'src/app/card-types/card-type.service';
import { AuthService } from 'src/app/auth/auth.service';
import { SessionInfoStorageService } from 'src/app/core/services/session-info-storage.service';
import { ColumnSettings, GridSettings, StatePersistingService } from 'src/app/shared/services/state-persisting.service';

@Component({
  selector: 'app-partner-customer-list',
  templateUrl: './partner-customer-list.component.html',
  styleUrls: ['./partner-customer-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PartnerCustomerListComponent implements OnInit {
  public gridSettings: GridSettings = {
    columnsConfig: [{
      field: 'phone',
      title: 'S??? ??i??n tho???i',
      _width: 130
    }, {
      field: 'dateOfBirth',
      title: 'Ng??y sinh',
      _width: 130,
    }, {
      field: 'age',
      title: 'Tu???i',
      _width: 100,
    }, {
      field: 'saleOrderDate',
      title: 'Nga??y ??i????u tri?? g????n nh????t',
      _width: 150,
      format: '{0:d}',
    }, {
      field: 'appointmentDate',
      title: 'Nga??y he??n g????n nh????t',
      _width: 150,
      format: '{0:d}',
    }, {
      field: 'orderState',
      title: 'T??nh tr???ng ??i???u tr???',
      _width: 150,
    }, {
      field: 'orderResidual',
      title: 'D??? ki???n thu',
      _width: 150,
    }, {
      field: 'totalDebit',
      title: 'C??ng n???',
      _width: 150,
    }, {
      field: 'cardTypeName',
      title: 'Th??? th??nh vi??n',
      _width: 150,
    }, {
      field: 'categories',
      title: 'Nh??n kh??ch h??ng',
      _width: 150,
    }, {
      field: 'companyName',
      title: 'Chi nh??nh t???o',
      _width: 150,
    }]
  };

  gridData: GridDataResult;
  filter = new PartnerInfoFilter();
  loading = false;
  opened = false;

  filteredCategs: PartnerCategoryBasic[];
  searchUpdate = new Subject<string>();

  @ViewChild("categMst", { static: true }) categMst: MultiSelectComponent;
  @ViewChild('popOver', { static: true }) public popover: NgbPopover;
  @ViewChild('cbxLevel', { static: true }) public cbxLevel: ComboBoxComponent;
  @ViewChild('cbxMember', { static: true }) public cbxMember: ComboBoxComponent;


  canExport = false;
  canAdd = false;
  canImport = false;
  canFilterPartnerCategory = false;
  canUpdateExcel = false;

  OrderResiduals: { text: string, value: boolean }[] = [
    { text: 'C?? d??? ki???n thu', value: true },
    { text: 'Kh??ng c?? d??? ki???n thu', value: false }
  ];

  totalDebits: { text: string, value: boolean }[] = [
    { text: 'C?? c??ng n???', value: true },
    { text: 'Kh??ng c?? c??ng n???', value: false }
  ];
  memberLevels = [];
  orderStateDisplay = {
    'sale': '??ang ??i???u tr???',
    'done': 'Ho??n th??nh',
    'draft': 'Ch??a ph??t sinh'
  };
  filterState = [
    { name: '??ang theo d??i', value: true },
    { name: 'Ng??ng theo d??i', value: false }
  ];

  memberCards = [];

  cbxPopupSettings = {
    width: 'auto'
  };

  color = 'red';

  showInfo = false;
  pagerSettings: any;
  visibleColumns: string[] = [];
  columnMenuItems: any[] = [
    {
      text: 'S??? ??i??n tho???i',
      field: 'phone'
    },
    {
      text: 'Ng??y sinh',
      field: 'birthday'
    },
    {
      text: 'Tu???i',
      field: 'age'
    },
    {
      text: 'Ng??y h???n g???n nh???t',
      field: 'appointmentDate'
    },
    {
      text: 'Ng??y ??i???u tr??? g???n nh???t',
      field: 'saleOrderDate'
    },
    {
      text: 'T??nh tr???ng ??i???u tr???',
      field: 'orderState'
    },
    {
      text: 'D??? ki???n thu',
      field: 'orderResidual'
    },
    {
      text: 'C??ng n???',
      field: 'debt'
    },
    {
      text: 'Th??? th??nh vi??n',
      field: 'cardType'
    },
    {
      text: 'Nh??n kh??ch h??ng',
      field: 'categPartner'
    },
    {
      text: 'Chi nh??nh t???o',
      field: 'companyName'
    }
  ]

  constructor(private partnerService: PartnerService, private modalService: NgbModal,
    private partnerCategoryService: PartnerCategoryService, private notificationService: NotificationService,
    private checkPermissionService: CheckPermissionService,
    private memberLevelService: MemberLevelService,
    private cardService: CardTypeService,
    private persistingService: StatePersistingService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig,
    private authService: AuthService,
    private sessionInfoStorageService: SessionInfoStorageService,
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.initFilter();
    this.refreshData();
    this.checkRole();
    this.loadCardTypes();
    this.cbxMember.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.cbxMember.loading = true)),
        switchMap((value) => this.searchCardTypes(value)
        )
      )
      .subscribe((x) => {
        this.memberCards = x.items;
        this.cbxMember.loading = false;
      });
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.refreshData();
        this.filter.offset = 0;
      });

    if (this.canFilterPartnerCategory && this.categMst) {
      this.categMst.filterChange
        .asObservable()
        .pipe(
          debounceTime(300),
          tap(() => (this.categMst.loading = true)),
          switchMap((value) => this.searchCategories(value))
        )
        .subscribe((result) => {
          this.filteredCategs = result;
          this.categMst.loading = false;
        });
    }

    if (this.canFilterPartnerCategory) {
      this.loadFilteredCategs();
    }

    if (localStorage.getItem('partners_grid_visible_columns')) {
      this.visibleColumns = this.persistingService.get('partners_grid_visible_columns');
    } else {
      this.visibleColumns = ['phone', 'birthday', 'age', 'orderState', 'orderResidual', 'debt', 'cardType', 'categPartner'];
      this.persistingService.set('partners_grid_visible_columns', this.visibleColumns);
    }
  }

  onCheckColumn(e) {
    var field = e.target.attributes["data-field"].value;
    if (e.target.checked) {
      this.visibleColumns.push(field);
    } else {
      var index = this.visibleColumns.indexOf(field);
      if (index !== -1) {
        this.visibleColumns.splice(index, 1);
      }
    }

    //save to localstorage
    this.persistingService.set('partners_grid_visible_columns', this.visibleColumns);
  }


  public saveGridSettings(grid: GridComponent): void {
    const columns = grid.columns;

    //add only the required column properties to save local storage space
    const gridConfig = {
      columnsConfig: columns.toArray().map((item) => {
        return <ColumnSettings>{
          field: item["field"],
          width: item["width"],
          _width: item["_width"],
          title: item["title"],
          filter: item["filter"],
          format: item["format"],
          filterable: item["filterable"],
          orderIndex: item["orderIndex"],
        };
      }),
    };

    this.persistingService.set("partners_grid_visible_columns", gridConfig);
  }


  initFilter() {
    this.filter.limit = 20;
    this.filter.offset = 0;
    this.filter.orderState = '';
    this.filter.isActive = true;
  }

  refreshData() {
    var val = Object.assign({}, this.filter);
    this.loading = true;
    this.partnerService.getPartnerInfoPaged2(val).subscribe(res => {
      this.gridData = <GridDataResult>{
        data: res.items,
        total: res.totalItems
      };
    }, () => { },
      () => {
        this.loading = false;
      }
    );
  }

  toggleTagsPopOver(popover: any, dataItem: any) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      popover.open({ dataItem });
    }
  }

  // searchMemberLevel(s?) {
  //   var val = new MemberLevelAutoCompleteReq();
  //   val.offset = 0;
  //   val.limit = 20;
  //   val.search = s || '';
  //   return this.memberLevelService.autoComplete(val);
  // }

  // loadMemberLevel(){
  //   this.searchMemberLevel().subscribe(res => {
  //     this.memberLevels = res.items;
  //   });
  // }
  loadCardTypes() {
    this.searchCardTypes().subscribe(result => {
      this.memberCards = result.items;

    })
  }

  searchCardTypes(q?: string) {
    var val = { search: q || '', offset: 0, limit: 10 };
    return this.cardService.getPaged(val);
  }
  importFromExcel() {
    const modalRef = this.modalService.open(PartnerImportComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.type = 'customer';
    modalRef.componentInstance.title = 'Import Excel'
    modalRef.result.then(() => {
      this.refreshData();
    }, () => {
    });
  }

  updateFromExcel() {
    const modalRef = this.modalService.open(PartnerImportComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.type = 'customer';
    modalRef.componentInstance.title = 'C???p nh???t Excel';
    modalRef.componentInstance.isUpdate = true;
    modalRef.result.then(() => {
      this.refreshData();
    }, () => {
    });
  }

  loadFilteredCategs() {
    this.searchCategories().subscribe(
      (result) => (this.filteredCategs = result)
    );
  }

  onCategChange(value) {
    this.filter.categIds = value.map(x => x.id);
    this.filter.offset = 0;
    this.refreshData();
  }

  searchCategories(search?: string) {
    var val = new PartnerCategoryPaged();
    val.search = search;
    return this.partnerCategoryService.autocomplete(val);
  }

  exportPartnerExcelFile() {
    var val = Object.assign({}, this.filter);
    this.partnerService.exportPartnerExcelFile(val).subscribe((rs) => {
      let filename = "danh_sach_khach_hang";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  createItem() {
    const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Th??m kh??ch h??ng';
    modalRef.result.then(() => {
      this.refreshData();
    }, er => { })
  }

  editItem(item: any) {
    const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a kh??ch h??ng';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.refreshData();
    }, () => {
    })
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a kh??ch h??ng';
    modalRef.componentInstance.body = 'B???n c?? mu???n x??a kh??ch h??ng kh??ng?';

    modalRef.result.then(() => {
      this.partnerService.delete(item.id).subscribe(() => {
        this.notificationService.show({
          content: 'X??a kh??ch h??ng th??nh c??ng',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.refreshData();
      }, () => {
      });
    }, () => {
    });
  }

  setupAutoCodeCustomer() {
    let modalRef = this.modalService.open(PartnerCustomerAutoGenerateCodeDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.code = 'customer';

    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'C???p nh???t th??nh c??ng',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    }, () => {
    });
  }

  checkRole() {
    this.canExport = this.checkPermissionService.check(['Basic.Partner.Export']);
    this.canAdd = this.checkPermissionService.check(['Basic.Partner.Create']);
    this.canImport = this.checkPermissionService.check(['Basic.Partner.Create']);
    this.canFilterPartnerCategory = this.checkPermissionService.check(["Catalog.PartnerCategory.Read"])
    this.canUpdateExcel = this.checkPermissionService.check(["Basic.Partner.Update"]);
    this.showInfo = this.checkPermissionService.check(["Basic.Partner.ContactInfo"]);
  }

  onResidualSelect(e) {
    this.filter.isRevenueExpect = e ? e.value : '';
    this.filter.offset = 0;
    this.refreshData();
  }

  onDebitSelect(e) {
    this.filter.isDebt = e ? e.value : '';
    this.filter.offset = 0;
    this.refreshData();
  }

  // onMemberLevelSelect(e)
  // {
  //   this.filter.memberLevelId = e? e.id : '';
  //   this.filter.offset = 0;
  //   this.refreshData();
  // }

  onMemberSelect(e) {
    this.filter.cardTypeIds = e ? e.id : [];
    this.filter.offset = 0;
    this.refreshData();
  }

  public onPageChange(event: PageChangeEvent): void {
    this.filter.offset = event.skip;
    this.filter.limit = event.take;
    this.refreshData();
  }

  onClickActive(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = `${!item.active ? 'Theo d??i' : 'Ng??ng theo d??i'} kh??ch h??ng ${item.name}`;
    modalRef.componentInstance.body = `B???n c?? ch???c ch???n mu???n ${!item.active ? '' : 'ng??ng'} theo d??i kh??ch h??ng n??y?`;
    modalRef.result.then(() => {
      var active = item.active ? false : true;
      if (active) {
        this.partnerService.actionUnArchive([item.id]).subscribe(() => {
          this.notificationService.show({
            content: 'C???p nh???t th??nh c??ng',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.refreshData();
        });
      } else {
        this.partnerService.actionArchive([item.id]).subscribe(() => {
          this.notificationService.show({
            content: 'C???p nh???t th??nh c??ng',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.refreshData();
        });
      }
    }, () => {
    });
  }

  onStateSelect(e) {
    this.filter.isActive = e ? e.value : '';
    this.filter.offset = 0;
    this.refreshData()
  }
}
