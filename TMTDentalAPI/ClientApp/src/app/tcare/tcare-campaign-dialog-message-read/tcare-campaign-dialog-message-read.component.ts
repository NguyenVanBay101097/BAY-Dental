import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FacebookTagsService, FacebookTagsPaged, FacebookTagBasic } from 'src/app/socials-channel/facebook-tags.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { PartnerCategoryBasic, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';

@Component({
  selector: 'app-tcare-campaign-dialog-message-read',
  templateUrl: './tcare-campaign-dialog-message-read.component.html',
  styleUrls: ['./tcare-campaign-dialog-message-read.component.css']
})
export class TcareCampaignDialogMessageReadComponent implements OnInit {

  @ViewChild('tagCbx', { static: true }) tagCbx: ComboBoxComponent
  title: string;
  formGroup: FormGroup;
  filterdPartnerCategoies: PartnerCategoryBasic[] = []
  listPartnerCategories: PartnerCategoryBasic[] = [];
  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private partnerCatgService: PartnerCategoryService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      tag: null
    });

    this.tagCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.tagCbx.loading = true)),
      switchMap(value => this.searchPartnerCategories(value))
    ).subscribe(result => {
      this.filterdPartnerCategoies = result.items;
      this.tagCbx.loading = false;
    });

    this.loadPartnerCategory();
  }

  searchPartnerCategories(q?: string) {
    var val = new FacebookTagsPaged();
    val.limit = 10;
    val.offset = 0;
    val.search = q || '';
    return this.partnerCatgService.getPaged(val);
  }

  addPartnerCategory() {
    if (this.formGroup.invalid)
      return false;
    var partnerCategory = new PartnerCategoryBasic();
    var index = this.listPartnerCategories.findIndex(x => x.id == this.formGroup.value.tag.id);
    if (index >= 0)
      return false;
    partnerCategory.id = this.formGroup.value.tag.id;
    partnerCategory.name = this.formGroup.value.tag.name;
    this.listPartnerCategories.push(partnerCategory);
    this.formGroup.get('tag').patchValue(null);
  }

  removePartnerCategory(tag) {
    var index = this.listPartnerCategories.findIndex(x => x.id == tag.id);
    if (index >= 0) {
      this.listPartnerCategories.splice(index, 1);
    }
  }

  loadPartnerCategory() {
    return this.searchPartnerCategories().subscribe(
      result => {
        this.filterdPartnerCategoies = result.items;
      }
    );
  }

  onSave() {
    this.activeModal.close(this.listPartnerCategories);
  }

}
