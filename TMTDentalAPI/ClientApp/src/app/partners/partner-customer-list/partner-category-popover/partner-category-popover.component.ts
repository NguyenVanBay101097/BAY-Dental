import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { Observable, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PartnerCategoryDisplay, PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { PartnerAddRemoveTags, PartnerService } from '../../partner.service';

@Component({
  selector: 'app-partner-category-popover',
  templateUrl: './partner-category-popover.component.html',
  styleUrls: ['./partner-category-popover.component.css']
})
export class PartnerCategoryPopoverComponent implements OnInit {
  value_partnerCategoryPopOver: any;
  partnerCategoriesPopover: any;
  search_partnerCategory: string;
  searchUpdatePopOver = new Subject<string>();

  @Input() rowPartnerId: string;
  @Output() onSave = new EventEmitter();

  @Output() otherOutput = new EventEmitter();
  @Input() otherInput: any;

  @ViewChild('popOver', { static: true }) public popover: NgbPopover;

  constructor(
    private partnerCategoryService: PartnerCategoryService,
    private partnerService: PartnerService
  ) { }

  ngOnInit() {
    this.searchUpdatePopOver.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadPartnerCategoryPopOver();
      });
    this.loadPartnerCategoryPopOver();
  }

  loadPartnerCategoryPopOver() {
    const val = new PartnerCategoryPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = this.search_partnerCategory || '';

    this.partnerCategoryService.getPaged(val).subscribe(res => {
      this.partnerCategoriesPopover = res.items;
      if (this.popover && this.popover.isOpen()) {
        this.popover.open({ partnerCategoriesPopOver: this.partnerCategoriesPopover });
      }
    }, err => {
      console.log(err);
    });
  }

  onToggleCategory(popOver) {
    if (this.otherInput) {
      this.otherInput.close();
    }

    if (popOver.isOpen()) {
      popOver.close();
    } else {
      const val = new PartnerCategoryPaged();
      val.limit = 20;
      val.offset = 0;
      val.partnerId = this.rowPartnerId;
      this.partnerCategoryService.getPaged(val).subscribe((res) => {
        this.value_partnerCategoryPopOver = res.items;
      });
      popOver.open({ partnerCategoriesPopOver: this.partnerCategoriesPopover });
      this.popover = popOver;
      this.otherOutput.emit(popOver);
    }
  }

  SavePartnerCategories() {
    console.log(this.value_partnerCategoryPopOver);
    const val = new PartnerAddRemoveTags();
    val.id = this.rowPartnerId;
    val.tagIds = [];
    this.value_partnerCategoryPopOver.forEach(element => {
      val.tagIds.push(element.id);
    });
    this.partnerService.updateTags(val).subscribe(() => {
      // this.loadDataFromApi();
      this.value_partnerCategoryPopOver = [];
      this.onSave.emit(val);
    });
  }

  handleFilterCategoryPopOver(value) {
    this.search_partnerCategory = value;
  }

  public onSizeChange(value) {
    console.log(value);
    console.log(this.value_partnerCategoryPopOver);
  }

  public valueNormalizer = (text$: Observable<string>) => text$.pipe(map((name: string) => {
    // search for matching item to avoid duplicates

    // search in values
    const matchingValue: any = this.value_partnerCategoryPopOver.find((item: any) => {
      return item['name'].toLowerCase() === name.toLowerCase();
    });

    if (matchingValue) {
      return matchingValue; // return the already selected matching value and the component will remove it
    }

    // search in data
    const matchingItem: any = this.partnerCategoriesPopover.find((item: any) => {
      return item['name'].toLowerCase() === name.toLowerCase();
    });

    if (matchingItem) {
      return matchingItem;
    } else {
      const val = new PartnerCategoryDisplay();
      val.name = name;
      this.partnerCategoryService.create(val).subscribe((res: any) => {
        const val2 = new PartnerAddRemoveTags();
        val2.id = this.rowPartnerId;
        val2.tagIds = [];
        val2.tagIds.push(res.id);
        this.value_partnerCategoryPopOver.push(res);
      });
      // return {
      //     id: Math.random(), //generate unique value for the custom item
      //     name: name
      // };
    }
  }))
}
