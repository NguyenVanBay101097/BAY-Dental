import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { FormBuilder } from '@angular/forms';
import { PartnerSourceSimple } from './../../partners/partner-simple';
import { CardTypeBasic, CardTypeService, CardTypePaged } from './../../card-types/card-type.service';
import { PartnerCategoryBasic, PartnerCategoryService, PartnerCategoryPaged } from './../../partner-categories/partner-category.service';
import { FormGroup } from '@angular/forms';
import { MultiSelectComponent, PopupSettings } from '@progress/kendo-angular-dropdowns';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { PartnerSourcePaged, PartnerSourceService } from './../../partner-sources/partner-source.service';
import { Component, OnInit, Input, Output, ViewChild, EventEmitter, SimpleChanges } from '@angular/core';
import * as _ from 'lodash';

@Component({
  selector: 'app-partner-treatment-report-filter-popup',
  templateUrl: './partner-treatment-report-filter-popup.component.html',
  styleUrls: ['./partner-treatment-report-filter-popup.component.css']
})
export class PartnerTreatmentReportFilterPopupComponent implements OnInit {
  @Input() filter: any;
  @Input() placement: any;
  @Output() valueChange = new EventEmitter();
  @ViewChild('partnerCategoryMultiSelect', { static: false }) partnerCategoryMultiSelect: MultiSelectComponent;
  @ViewChild('partnerSourcesMultiSelect', { static: false }) partnerSourcesMultiSelect: MultiSelectComponent;
  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;

  formGroup: FormGroup;

  listPartnerCategory: PartnerCategoryBasic[] = [];
  listPartnerSource: PartnerSourceSimple[] = [];
  popupSettings: PopupSettings = {
    appendTo: "component",
  };
  listGender: { text: string, value: string }[] = [
    { text: 'Nam', value: 'male' },
    { text: 'Nữ', value: 'female' },
    { text: 'Khác', value: 'other' },
  ];
  partnerTypes = [
    { text: 'Khách mới', value: 'new' },
    { text: 'Khách quay lại', value: 'old' }
  ];

  rangeDays: any = [
    { text: '> 1 tháng', intervalNbr: 1, interval: 'month' },
    { text: '> 3 tháng', intervalNbr: 3, interval: 'month' },
    { text: '> 6 tháng', intervalNbr: 6, interval: 'month' },
    { text: '> 12 tháng', intervalNbr: 12, interval: 'month' },
  ];
  constructor(
    private fb: FormBuilder,
    private partnerCategoryService: PartnerCategoryService,
    private partnerSourceService: PartnerSourceService,
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.formGroup && this.filter) {
      this.formGroup.patchValue(this.filter);
    }
  }

  ngOnInit(): void {
    this.formGroup = this.fb.group({
      categs: this.filter?.categs || [null],
      partnerSources: this.filter?.partnerSources || [null],
      typeReport: this.filter?.typeReport || null,
      gender: this.filter?.gender || null,
      overIntervalData: null
    });

    setTimeout(() => {
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
    if (this.filter) {
      this.formGroup.patchValue(this.filter);
    }
  }

}
