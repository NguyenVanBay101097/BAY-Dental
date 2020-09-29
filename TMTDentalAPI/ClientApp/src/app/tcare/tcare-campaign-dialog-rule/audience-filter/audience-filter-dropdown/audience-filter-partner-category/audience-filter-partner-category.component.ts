import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { Subject } from 'rxjs';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { PartnerCategoryService, PartnerCategoryPaged } from 'src/app/partner-categories/partner-category.service';

@Component({
  selector: 'app-audience-filter-partner-category',
  templateUrl: './audience-filter-partner-category.component.html',
  styleUrls: ['./audience-filter-partner-category.component.css']
})

export class AudienceFilterPartnerCategoryComponent implements OnInit {
  
  formGroup: FormGroup;
  filteredCategs = [];
  submitted = false;
  type: string;
  name: string;
  @Output() saveClick = new EventEmitter<any>();
  @ViewChild('categMst', { static: true }) categMst: MultiSelectComponent;
  data: any;

  constructor(private fb: FormBuilder, 
    private partnerCategoryService: PartnerCategoryService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      op: 'contains',
      categ: [null, Validators.required]
    });

    setTimeout(() => {
      if (this.data) {
        var categ = this.data.list;

        this.formGroup.patchValue({
          op: this.data.op,
          categ: categ
        });

        this.filteredCategs.push(categ);
      }

      this.loadFilteredCategs();

      this.categMst.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.categMst.loading = true)),
        switchMap(value => this.searchCategories(value))
      ).subscribe((result: any) => {
        this.filteredCategs = result;
        this.categMst.loading = false;
      });
    });
  }

  loadFilteredCategs() {
    this.searchCategories().subscribe(result => {
      this.filteredCategs = _.unionBy(this.filteredCategs, result, 'id');
    });
  }

  searchCategories(q?: string) {
    var val = new PartnerCategoryPaged();
    val.search = q || '';
    return this.partnerCategoryService.autocomplete(val);
  }

  getOpDisplay(op) {
    switch (op) {
      case 'contains':
        return 'Chứa';
      case 'not_contains':
        return 'Không chứa';
      default:
        return '';
    }
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    var res = {
      type: this.type,
      op: value.op,
      name: this.name + " " + this.getOpDisplay(value.op) + " ",
      list: []
    };
    for (let i = 0; i < value.categ.length; i++) {
      res.list.push({ id: value.categ[i].id, name: value.categ[i].name });
      res.name += value.categ[i].name;
      if (i != value.categ.length - 1)
        res.name += ", ";
      else 
        res.name += ". ";
    }

    this.saveClick.emit(res);
  }
}
