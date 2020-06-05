import { Component, OnInit, ViewChild } from "@angular/core";
import { GridDataResult, PageChangeEvent } from "@progress/kendo-angular-grid";
import { ProductService, ProductPaged } from "../product.service";
import { ProductDialogComponent } from "../product-dialog/product-dialog.component";
import { Product } from "../product";
import { IntlService } from "@progress/kendo-angular-intl";
import {
  map,
  debounceTime,
  distinctUntilChanged,
  tap,
  switchMap,
} from "rxjs/operators";
import { Subject } from "rxjs";
import {
  ProductCategoryBasic,
  ProductCategoryService,
  ProductCategoryPaged,
} from "src/app/product-categories/product-category.service";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { ProductImportExcelDialogComponent } from "../product-import-excel-dialog/product-import-excel-dialog.component";
import { ActivatedRoute } from "@angular/router";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ConfirmDialogComponent } from "src/app/shared/confirm-dialog/confirm-dialog.component";
import { ProductServiceCuDialogComponent } from "../product-service-cu-dialog/product-service-cu-dialog.component";
import { ProductServiceImportDialogComponent } from "../product-service-import-dialog/product-service-import-dialog.component";

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
    private route: ActivatedRoute,
    private modalService: NgbModal
  ) {}

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
      size: "lg",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
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
      size: "lg",
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
      size: "lg",
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
        this.productService.delete(item.id).subscribe(
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
