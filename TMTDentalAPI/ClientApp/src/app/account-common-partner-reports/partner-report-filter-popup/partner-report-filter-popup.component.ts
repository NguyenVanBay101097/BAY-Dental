import { AfterViewInit, Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MultiSelectComponent, PopupSettings } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { debounceTime, switchMapTo, tap } from 'rxjs/operators';
import { CardTypeBasic, CardTypePaged, CardTypeService } from 'src/app/card-types/card-type.service';
import { PartnerCategoryBasic, PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { PartnerSourcePaged, PartnerSourceService } from 'src/app/partner-sources/partner-source.service';
import { PartnerSourceSimple } from 'src/app/partners/partner-simple';
import { AccountCommonPartnerReportOverviewFilter } from '../account-common-partner-report.service';

@Component({
  selector: 'app-partner-report-filter-popup',
  templateUrl: './partner-report-filter-popup.component.html',
  styleUrls: ['./partner-report-filter-popup.component.css']
})
export class PartnerReportFilterPopupComponent implements OnInit, AfterViewInit {
  @Output() onCloseEmit = new EventEmitter();
  @Output() onApplyEmit = new EventEmitter();
  @ViewChild('partnerCategoryMultiSelect', { static: false }) partnerCategoryMultiSelect: MultiSelectComponent;
  @ViewChild('partnerSourcesMultiSelect', { static: false }) partnerSourcesMultiSelect: MultiSelectComponent;
  @ViewChild('cardTypesMultiSelect', { static: false }) cardTypesMultiSelect: MultiSelectComponent;

  formGroup: FormGroup;
  dataFilterObj = Object.create({});
  filter = new AccountCommonPartnerReportOverviewFilter();

  listPartnerCategory: PartnerCategoryBasic[] = [];
  listCardType: CardTypeBasic[] = [];
  listPartnerSource: PartnerSourceSimple[] = [];
  popupSettings: PopupSettings = {
    appendTo: "component",
  };

  constructor(
    private fb: FormBuilder,
    private partnerCategoryService: PartnerCategoryService,
    private cardTypeService: CardTypeService,
    private partnerSourceService: PartnerSourceService,

  ) { }

  ngOnInit(): void {
    this.formGroup = this.fb.group({
      categs: [null],
      partnerSources: [null],
      cardTypes: [null],
      ageFrom: null,
      ageTo: null,
      revenueFrom: null,
      revenueTo: null,
      revenueExpectFrom: null,
      revenueExpectTo: null,
      debtFrom: null,
      debtTo: null,
    });
    setTimeout(() => {
      this.loadListCardType();
      this.loadSourceList();
      this.loadPartnerCategoryList();
    }, 200);
  }

  ngAfterViewInit(): void {

  }

  // filterMultiSelect() {
  //   this.partnerCategoryMultiSelect.filterChange.asObservable().pipe(
  //     debounceTime(300),
  //     tap(() => (this.partnerCategoryMultiSelect.loading = true)),
  //     switchMapTo(value => this.searchPartnerCategory(value))
  //   ).subscribe((result: any) => {
  //     this.listPartnerCategory = result;
  //     this.partnerCategoryMultiSelect.loading = false;
  //   });
  // }

  loadPartnerCategoryList() {
    this.searchPartnerCategory().subscribe((result) => {
      this.listPartnerCategory = _.unionBy(this.listPartnerCategory, result, 'id');
    });
  }

  searchPartnerCategory(q?: string) {
    var val = new PartnerCategoryPaged();
    val.search = q || '';
    return this.partnerCategoryService.autocomplete(val);
  }

  loadListCardType() {
    this.searchCardTypes().subscribe(result => {
      this.listCardType = _.unionBy(this.listCardType, result, 'id');
    })
  }

  searchCardTypes(search?: string) {
    var val = new CardTypePaged();
    val.search = search || '';
    return this.cardTypeService.autoComplete(val);
  }

  loadSourceList() {
    this.searchSources().subscribe((result) => {
      this.listPartnerSource = _.unionBy(this.listPartnerSource, result, 'id');
    });
  }

  searchSources(q?: string) {
    var val = new PartnerSourcePaged();
    val.search = q;
    return this.partnerSourceService.autocomplete(val);
  }

  onApply() {
    this.dataFilterObj = { ...this.formGroup.value };
    this.onApplyEmit.emit(this.dataFilterObj);
  }

  onRemoveFilter(key: string) {
    if (key === 'age') {
      this.formGroup.get('ageFrom').setValue(null);
      this.formGroup.get('ageTo').setValue(null);
    }
    else if (key === 'revenue') {
      this.formGroup.get('revenueFrom').setValue(null);
      this.formGroup.get('revenueTo').setValue(null);
    }
    else if (key === 'revenueExpect') {
      this.formGroup.get('revenueExpectFrom').setValue(null);
      this.formGroup.get('revenueExpectTo').setValue(null);
    }
    else if (key === 'debt') {
      this.formGroup.get('debtFrom').setValue(null);
      this.formGroup.get('debtTo').setValue(null);
    } else {
      this.formGroup.get(key).setValue(null);
    }
    this.onApply();
  }

  onUpdateFormValue() {
    this.formGroup.patchValue(this.dataFilterObj);
  }

  onRemoveAllFilters() {
    this.formGroup.reset();
  }

  onClose() {
    this.onCloseEmit.emit(true);
  }
}
