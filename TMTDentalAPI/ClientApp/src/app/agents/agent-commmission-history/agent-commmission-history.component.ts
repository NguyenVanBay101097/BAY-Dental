import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PartnerService } from 'src/app/partners/partner.service';
import { PhieuThuChiPaged, PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { PrintService } from 'src/app/shared/services/print.service';

@Component({
  selector: 'app-agent-commmission-history',
  templateUrl: './agent-commmission-history.component.html',
  styleUrls: ['./agent-commmission-history.component.css']
})
export class AgentCommmissionHistoryComponent implements OnInit {
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  agentId : string;
  search: string;
  limit = 20;
  offset = 0;
  edit = false;
  dateFrom: Date;
  dateTo: Date;
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  
  constructor(private intlService: IntlService,
    private modalService: NgbModal,
    private partnerService: PartnerService,
    private phieuthuchiService: PhieuThuChiService,
    private router: Router,
    private printService: PrintService,
    private route: ActivatedRoute,
    private notifyService: NotifyService) { }

  ngOnInit() {
    this.agentId = this.route.parent.snapshot.paramMap.get('id');
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.offset = 0;
        this.loadDataFromApi();
      });


    this.loadDataFromApi();
  }

  
  loadDataFromApi(){
    this.loading = true;
    var paged = new PhieuThuChiPaged();
    paged.limit = this.limit;
    paged.offset = this.offset;
    paged.agentId = this.agentId;
    paged.accountType = 'commission';
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.phieuthuchiService.getPaged(paged).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;

      this.loading = false;
    }, err => {
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.offset = 0;
    this.loadDataFromApi();
  }

  printItem(item) {
    this.phieuthuchiService.getPrint2(item.id).subscribe((data: any) => {
      this.printService.printHtml(data.html);
    });
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa phiếu chi hoa hồng';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa phiếu chi hoa hồng ?';
    modalRef.result.then(() => {
      this.phieuthuchiService.delete(item.id).subscribe(() => {
        this.notifyService.notify('success','Xóa thành công');
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }

}
