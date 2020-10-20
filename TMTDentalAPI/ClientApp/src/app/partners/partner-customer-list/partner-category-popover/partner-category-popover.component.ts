import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { Observable, Subject } from 'rxjs';
import { of } from 'rxjs/internal/observable/of';
import { debounceTime, distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { PartnerCategoryDisplay, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { PartnerCategoriesService } from 'src/app/shared/services/partner-categories.service';
import { PartnerAddRemoveTags, PartnerService } from '../../partner.service';

@Component({
  selector: 'app-partner-category-popover',
  templateUrl: './partner-category-popover.component.html',
  styleUrls: ['./partner-category-popover.component.css']
})
export class PartnerCategoryPopoverComponent implements OnInit {
  value_partnerCategoryPopOver: any;
  @Input() tags = [];
  @Input() dataPopOver = [];
  search_partnerCategory: string;
  searchUpdatePopOver = new Subject<string>();

  @Input() rowPartnerId: string;
  @Output() onSave = new EventEmitter();

  @Output() otherOutput = new EventEmitter();
  @Input() otherInput: any;

  @ViewChild('popOver', { static: true }) public popover: any;
  @ViewChild("list", { static: true }) public list: MultiSelectComponent;

  constructor(
    private partnerCategoriesService: PartnerCategoriesService,
    private partnerCategoryService: PartnerCategoryService,
    private partnerService: PartnerService
  ) { }

  ngOnInit() {
    this.loadPartnerCategoryPopOver();

    // this.searchUpdatePopOver.pipe(
    //   debounceTime(300),
    //   distinctUntilChanged())
    //   .subscribe((value) => {
    //     this.loadPartnerCategoryPopOver(value);
    //   });
  }

  loadPartnerCategoryPopOver(q?: string) {
    this.partnerCategoriesService.searchCombobox(q).subscribe((res: any) => {
      this.dataPopOver = res;
    }, err => {
      console.log(err);
    });
  }

  onToggleCategory(popOver) {
    if (this.otherInput && this.otherInput !== popOver) {
      this.otherInput.close();
      this.value_partnerCategoryPopOver = [];
    }

    if (popOver.isOpen()) {
      popOver.close();
      this.value_partnerCategoryPopOver = [];

    } else {
      // this.getValueDefault();
      this.loadPartnerCategoryPopOver();
      popOver.open();
      this.popover = popOver;
      this.otherOutput.emit(popOver);
    }
  }

  // getValueDefault() {
  //   const val = new PartnerCategoryPaged();
  //   val.limit = 20;
  //   val.offset = 0;
  //   val.partnerId = this.rowPartnerId;
  //   this.partnerCategoryService.getPaged(val).subscribe((res) => {
  //     this.value_partnerCategoryPopOver = res.items;
  //   });
  // }

  SavePartnerCategories() {
    console.log(this.value_partnerCategoryPopOver);
    const val = new PartnerAddRemoveTags();
    val.id = this.rowPartnerId;
    val.tagIds = [];
    this.tags.forEach(element => {
      val.tagIds.push(element.Id);
    });
    this.partnerService.updateTags(val).subscribe(() => {
      // this.loadDataFromApi();
      this.value_partnerCategoryPopOver = [];
      this.onToggleCategory(this.popover);
      // this.onSave.emit(val);
    });
  }

  handleFilterCategoryPopOver(value) {
    this.search_partnerCategory = value;
  }

  public valueNormalizer = (text$: Observable<string>): any => text$.pipe(
    switchMap((text: string) => {
      // Search in values
      const matchingValue: any = this.tags.find((item: any) => {
        // Search for matching item to avoid duplicates
        return item['Name'].toLowerCase() === text.toLowerCase();
      });

      if (matchingValue) {
        // Return the already selected matching value and the component will remove it
        return of(matchingValue);
      }

      // Search in data
      const matchingItem: any = this.dataPopOver.find((item: any) => {
        return item['Name'].toLowerCase() === text.toLowerCase();
      });

      if (matchingItem) {
        return of(matchingItem);
      } else {
        return of(text).pipe(switchMap(this.service$));
      }
    })
  )

  public service$ = (text: string): any => {
    const val = new PartnerCategoryDisplay();
    val.name = text;
    return this.partnerCategoryService.create(val)
      .pipe(
        map((result: any) => {
          return {
            Id: result.id,
            Name: result.name
          }
        })
      );
  }
}
