import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { Subject } from 'rxjs';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
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
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  data: any;

  constructor(private fb: FormBuilder, 
    private partnerCategoryService: PartnerCategoryService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      op: 'contains',
      tag: [null, Validators.required]
    });

    setTimeout(() => {
      if (this.data) {
        var tagId = this.data.tagId;
        var val2 = new PartnerCategoryPaged();
        val2.limit = 1;
        val2.ids = [tagId];
        this.partnerCategoryService.getPaged(val2).subscribe(result => {
          var item = result.items.length ? result.items[0] : null;
          var d = {
            op: this.data.op,
            tag: item
          };
          this.formGroup.patchValue(d);

          if (item) {
            this.filteredCategs = _.unionBy(this.filteredCategs, [item], 'id');
          }
        });
      }

      this.loadFilteredCategs();

      this.categCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.categCbx.loading = true)),
        switchMap(value => this.searchCategories(value))
      ).subscribe((result: any) => {
        this.filteredCategs = result.items;
        this.categCbx.loading = false;
      });
    });
  }

  loadFilteredCategs() {
    this.searchCategories().subscribe(result => {
      this.filteredCategs = _.unionBy(this.filteredCategs, result.items, 'id');
    });
  }

  searchCategories(q?: string) {
    var val = new PartnerCategoryPaged();
    val.search = q || '';
    return this.partnerCategoryService.getPaged(val);
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
      name: this.name + " " + this.getOpDisplay(value.op) + " " + value.tag.name,
      tagId: value.tag.id
    };

    this.saveClick.emit(res);
  }
}
