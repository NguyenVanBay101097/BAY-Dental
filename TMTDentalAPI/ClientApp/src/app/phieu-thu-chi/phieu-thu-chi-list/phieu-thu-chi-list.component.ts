import { Component, Inject, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { loaiThuChiBasic } from 'src/app/loai-thu-chi/loai-thu-chi.service';
import { PhieuThuChiService, PhieuThuChiPaged } from '../phieu-thu-chi.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-phieu-thu-chi-list',
  templateUrl: './phieu-thu-chi-list.component.html',
  styleUrls: ['./phieu-thu-chi-list.component.css']
})
export class PhieuThuChiListComponent implements OnInit {
  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  type: string;

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute, private modalService: NgbModal,
    private phieuThuChiService: PhieuThuChiService, private router: Router,
    private printService: PrintService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.type = params.get('type');
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new PhieuThuChiPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = this.type;
    this.phieuThuChiService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      console.log(res);
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  stateGet(state) {
    switch (state) {
      case 'posted':
        return '???? x??c nh???n';
      default:
        return 'Nh??p';
    }
  }

  converttype() {
    switch (this.type) {
      case 'thu':
        return 'phi???u thu';
      case 'chi':
        return 'phi???u chi';
    }
  }

  getTypePayerReceiver() {
    switch (this.type) {
      case 'thu':
        return 'Ng?????i nh???n ti???n';
      case 'chi':
        return 'Ng?????i n???p ti???n';
    }
  }

  createItem() {
    this.router.navigate(['/phieu-thu-chi/form'], { queryParams: { type: this.type } });
  }

  editItem(item: any) {
    this.router.navigate(['/phieu-thu-chi/form'], { queryParams: { id: item.id, type: this.type } });
  }

  deleteItem(item: loaiThuChiBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a ' + this.converttype();

    modalRef.result.then(() => {
      this.phieuThuChiService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }

  printItem(id) {
    this.phieuThuChiService.getPrint(id).subscribe((data: any) => {
      this.printService.printHtml(data);
    });
  }
}
