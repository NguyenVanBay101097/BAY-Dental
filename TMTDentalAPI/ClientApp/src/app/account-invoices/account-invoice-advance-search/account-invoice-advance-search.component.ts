import { Component, OnInit, Output, EventEmitter, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ProductCategoryBasic, ProductCategoryService, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap, distinctUntilChanged, catchError } from 'rxjs/operators';
import { UserBasic, UserService, UserPaged } from 'src/app/users/user.service';
import { UserSimple } from 'src/app/users/user-simple';

@Component({
  selector: 'app-account-invoice-advance-search',
  templateUrl: './account-invoice-advance-search.component.html',
  styleUrls: ['./account-invoice-advance-search.component.css'],
  host: {
    class: "o_advance_search"
  }
})

export class AccountInvoiceAdvanceSearchComponent implements OnInit {
  formGroup: FormGroup;
  @Output() searchChange = new EventEmitter<any>();

  filteredUsers: UserSimple[];
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;

  show = false;
  defaultFormGroup = {
    draftState: false,
    openState: false,
    paidState: false,
    cancelState: false,
    user: null,
    dateOrderFrom: null,
    dateOrderTo: null
  };
  constructor(private fb: FormBuilder, private userService: UserService) { }

  ngOnInit() {
    this.formGroup = this.fb.group(this.defaultFormGroup);

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.userCbx.loading = true)),
      switchMap(value => this.searchCategories(value))
    ).subscribe(result => {
      this.filteredUsers = result;
      this.userCbx.loading = false;
    });

    this.loadFilteredCategs();
  }

  toggleShow() {
    this.show = !this.show;
  }

  onSearch() {
    this.searchChange.emit(this.formGroup.value);
  }

  onClear() {
    this.formGroup = this.fb.group(this.defaultFormGroup);
    this.searchChange.emit(this.formGroup.value);
  }

  searchCategories(search?: string) {
    var val = new UserPaged();
    val.search = search;
    return this.userService.autocompleteSimple(val);
  }

  loadFilteredCategs() {
    this.searchCategories().subscribe(result => this.filteredUsers = result);
  }
}


