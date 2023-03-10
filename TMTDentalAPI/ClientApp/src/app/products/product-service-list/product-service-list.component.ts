import { Component, Inject, OnInit, ViewChild } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { GridDataResult, PageChangeEvent } from "@progress/kendo-angular-grid";
import { IntlService } from "@progress/kendo-angular-intl";
import { Subject } from "rxjs";
import {
  debounceTime,
  distinctUntilChanged, map, switchMap, tap
} from "rxjs/operators";
import {
  ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService
} from "src/app/product-categories/product-category.service";
import { ConfirmDialogComponent } from "src/app/shared/confirm-dialog/confirm-dialog.component";
import { PageGridConfig, PAGER_GRID_CONFIG } from "src/app/shared/pager-grid-kendo.config";
import { Product } from "../product";
import { ProductImportExcelDialogComponent } from "../product-import-excel-dialog/product-import-excel-dialog.component";
import { ProductServiceCuDialogComponent } from "../product-service-cu-dialog/product-service-cu-dialog.component";
import { ProductPaged, ProductService } from "../product.service";

@Component({
  selector: "app-product-service-list",
  templateUrl: "./product-service-list.component.html",
  styleUrls: ["./product-service-list.component.css"],
  host: {
    class: "o_action o_view_controller",
  },
})
export class ProductServiceListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  opened = false;
  title = "Dịch vụ";

  search: string;
  searchCateg: ProductCategoryBasic;
  filteredCategs: ProductCategoryBasic[];
  searchUpdate = new Subject<string>();
  @ViewChild("categCbx", { static: true }) categCbx: ComboBoxComponent;

  constructor(
    private productService: ProductService,
    public intl: IntlService,
    private productCategoryService: ProductCategoryService,
    private modalService: NgbModal,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.loadDataFromApi();
      });

    this.categCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.categCbx.loading = true)),
        switchMap((value) => this.searchCategories(value))
      )
      .subscribe((result) => {
        this.filteredCategs = result;
        this.categCbx.loading = false;
      });

    this.loadDataFromApi();
    this.loadFilteredCategs();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || "";
    val.categId = this.searchCateg ? this.searchCateg.id : "";
    val.type2 = "service";

    this.productService
      .getPaged(val)
      .pipe(
        map(
          (response) =>
            <GridDataResult>{
              data: response.items,
              total: response.totalItems,
            }
        )
      )
      .subscribe(
        (res) => {
          this.gridData = res;

          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  loadFilteredCategs() {
    this.searchCategories().subscribe(
      (result) => (this.filteredCategs = result)
    );
  }

  onCategChange(value) {
    this.searchCateg = value;
    this.loadDataFromApi();
  }

  searchCategories(search?: string) {
    var val = new ProductCategoryPaged();
    val.search = search;
    val.type = "service";
    return this.productCategoryService.autocomplete(val);
  }

  importFromExcel() {
    let modalRef = this.modalService.open(ProductImportExcelDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
      scrollable: true
    });
    modalRef.componentInstance.title = "Import excel";
    modalRef.componentInstance.type = "service";
    modalRef.result.then(
      () => {
        this.loadDataFromApi();
      },
      () => {}
    );
  }

  exportExcelFile() {
    var paged = new ProductPaged();

    paged.search = this.search || "";
    paged.categId = this.searchCateg ? this.searchCateg.id : "";
    this.productService.excelServiceExport(paged).subscribe((rs) => {
      let filename = "ExportedExcelFile";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      console.log(rs);

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
    let modalRef = this.modalService.open(ProductServiceCuDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm: " + this.title;
    modalRef.result.then(
      () => {
        this.loadDataFromApi();
      },
      () => {}
    );
  }

  editItem(item: Product) {
    let modalRef = this.modalService.open(ProductServiceCuDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Sửa: " + this.title;
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(
      () => {
        this.loadDataFromApi();
      },
      () => {}
    );
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, {
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Xóa: " + this.title;

    modalRef.result.then(
      () => {
        this.productService.deleteService(item.id).subscribe(
          () => {
            this.loadDataFromApi();
          },
          (err) => {
            console.log(err);
          }
        );
      },
      () => {}
    );
  }
}
