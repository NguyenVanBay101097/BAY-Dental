import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { MultiSelectComponent, PopupSettings } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { CardTypeBasic, CardTypePaged, CardTypeService } from 'src/app/card-types/card-type.service';
import { PartnerCategoryBasic, PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { PartnerSourcePaged, PartnerSourceService } from 'src/app/partner-sources/partner-source.service';
import { PartnerSourceSimple } from 'src/app/partners/partner-simple';

@Component({
  selector: 'app-partner-report-filter-popup',
  templateUrl: './partner-report-filter-popup.component.html',
  styleUrls: ['./partner-report-filter-popup.component.css']
})
export class PartnerReportFilterPopupComponent implements OnInit, AfterViewInit, OnChanges {
  @Input() filter: any;
  @Input() placement: any;
  @Output() valueChange = new EventEmitter();
  @ViewChild('partnerCategoryMultiSelect', { static: false }) partnerCategoryMultiSelect: MultiSelectComponent;
  @ViewChild('partnerSourcesMultiSelect', { static: false }) partnerSourcesMultiSelect: MultiSelectComponent;
  @ViewChild('cardTypesMultiSelect', { static: false }) cardTypesMultiSelect: MultiSelectComponent;
  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;

  formGroup: FormGroup;

  listPartnerCategory: PartnerCategoryBasic[] = [];
  listCardType: CardTypeBasic[] = [];
  listPartnerSource: PartnerSourceSimple[] = [];
  popupSettings: PopupSettings = {
    appendTo: "component",
  };
  listGender: { text: string, value: string }[] = [
    { text: 'Nam', value: 'male' },
    { text: 'Nữ', value: 'female' },
    { text: 'Khác', value: 'other' },
  ]

  constructor(
    private fb: FormBuilder,
    private partnerCategoryService: PartnerCategoryService,
    private cardTypeService: CardTypeService,
    private partnerSourceService: PartnerSourceService,
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.formGroup) {
      this.formGroup.patchValue(this.filter);
    }
  }

  ngOnInit(): void {
    this.formGroup = this.fb.group({
      categs: this.filter?.categs || [null],
      partnerSources: this.filter?.partnerSources || [null],
      cardTypes: this.filter?.cardTypes || [null],
      ageFrom: this.filter?.ageFrom,
      ageTo: this.filter?.ageTo,
      revenueFrom: this.filter?.revenueFrom,
      revenueTo: this.filter?.revenueTo,
      amountTotalFrom: this.filter?.amountTotalFrom,
      amountTotalTo: this.filter?.amountTotalTo,
      gender: this.filter?.gender,
    });

    setTimeout(() => {
      this.loadListCardType();
      this.loadSourceList();
      this.loadPartnerCategoryList();
    }, 200);
  }

  ngAfterViewInit(): void {
    this.filterMultiSelect();
  }

  filterMultiSelect() {
    this.partnerCategoryMultiSelect.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerCategoryMultiSelect.loading = true)),
      switchMap(value => this.searchPartnerCategory(value))
    ).subscribe((result: any) => {
      this.listPartnerCategory = result;
      this.partnerCategoryMultiSelect.loading = false;
    });

    this.partnerSourcesMultiSelect.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerSourcesMultiSelect.loading = true)),
      switchMap(value => this.searchSources(value))
    ).subscribe((result: any) => {
      this.listPartnerSource = result;
      this.partnerSourcesMultiSelect.loading = false;
    });

    this.cardTypesMultiSelect.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.cardTypesMultiSelect.loading = true)),
      switchMap(value => this.searchCardTypes(value))
    ).subscribe((result: any) => {
      this.listCardType = result;
      this.cardTypesMultiSelect.loading = false;
    });
  }

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
    const dataFilterObj = { ...this.formGroup.value };
    this.valueChange.emit(dataFilterObj);
    this.myDrop.close();
  }

  onRemoveAllFilters() {
    this.formGroup.reset();
  }

  onClose() {
    this.myDrop.close();
  }

  onToggleDropdown(event?: boolean) {
    this.formGroup.patchValue(this.filter);
  }
}
