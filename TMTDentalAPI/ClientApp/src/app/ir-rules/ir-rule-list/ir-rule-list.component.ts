import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { IRRuleService } from '../ir-rule.service';
import { IRRulePaged, IRRuleBasic } from '../ir-rule';
import { IrRuleCuDialogComponent } from '../ir-rule-cu-dialog/ir-rule-cu-dialog.component';

@Component({
  selector: 'app-ir-rule-list',
  templateUrl: './ir-rule-list.component.html',
  styleUrls: ['./ir-rule-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})

export class IrRuleListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  title = 'Đối tượng';

  constructor(private ruleService: IRRuleService, private modalService: NgbModal) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }


  loadDataFromApi() {
    this.loading = true;
    var val = new IRRulePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    if (this.search) {
      val.filter = this.search;
    }

    this.ruleService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    let modalRef = this.modalService.open(IrRuleCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm ' + this.title;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: IRRuleBasic) {
    let modalRef = this.modalService.open(IrRuleCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa ' + this.title;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item: IRRuleBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa ' + this.title + ": " + item.name;
    modalRef.result.then(() => {
      this.ruleService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    }, () => {
    });
  }
}

