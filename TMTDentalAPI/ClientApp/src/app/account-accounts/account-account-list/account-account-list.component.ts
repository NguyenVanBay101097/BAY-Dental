import { Component, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AccountAccountFormComponent } from '../account-account-form/account-account-form.component';

@Component({
  selector: 'app-account-account-list',
  templateUrl: './account-account-list.component.html',
  styleUrls: ['./account-account-list.component.css']
})
export class AccountAccountListComponent implements OnInit {
  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  resultSelection: string;

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute, private modalService: NgbModal) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.resultSelection = params.get('result_selection');
      // this.loadDataFromApi();

    });
  }

  create() {
    const modalRef = this.modalService.open(AccountAccountFormComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm loại' + this.resultSelection;
    modalRef.componentInstance.isDoctor = true;
    modalRef.result.then(() => {
      
    });
  }

}
