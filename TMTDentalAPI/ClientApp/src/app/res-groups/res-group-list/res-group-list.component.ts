import { Component, ViewChild, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ResGroupService, ResGroupPaged, ResGroupBasic } from '../res-group.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-res-group-list',
  templateUrl: './res-group-list.component.html',
  styleUrls: ['./res-group-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})

export class ResGroupListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private resGroupService: ResGroupService, private dialogService: DialogService,
    private router: Router) {
  }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

  }

  resetData() {
    this.resGroupService.resetSecurityData().subscribe(() => {
      this.loadDataFromApi();
    });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ResGroupPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.resGroupService.getPaged(val).pipe(
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
    this.router.navigate(['res-groups/create']);
  }

  editItem(item: ResGroupBasic) {
    this.router.navigate(['res-groups/edit/', item.id]);
  }

  deleteItem(item) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'Xóa nhóm quyền',
      content: 'Bạn có chắc chắn muốn xóa?',
      actions: [
        { text: 'Hủy bỏ', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ],
      width: 450,
      height: 200,
      minWidth: 250
    });

    dialog.result.subscribe((result) => {
      if (result instanceof DialogCloseResult) {
        console.log('close');
      } else {
        console.log('action', result);
        if (result['value']) {
          this.resGroupService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }
}

