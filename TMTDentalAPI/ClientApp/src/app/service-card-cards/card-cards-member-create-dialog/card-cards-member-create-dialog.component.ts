import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { CardCardService } from 'src/app/card-cards/card-card.service';
import { CardTypeService } from 'src/app/card-types/card-type.service';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-card-cards-member-create-dialog',
  templateUrl: './card-cards-member-create-dialog.component.html',
  styleUrls: ['./card-cards-member-create-dialog.component.css']
})
export class CardCardsMemberCreateDialogComponent implements OnInit {
  @ViewChild('cardTypeCbx', { static: true }) cardTypeCbx: ComboBoxComponent;
  formGroup: FormGroup;
  submitted = false;
  cardTypeSimpleFilter: any[] = [];
  partnerId: string;
  title: string = '';

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private partnerService: PartnerService,
    private cardCardsService: CardCardService,
    private cardService: CardTypeService,
  ) { }

  ngOnInit(): void {
    this.formGroup = this.fb.group({
      barcode: ['', [Validators.required,createLengthValidator()]],
      type: [null,Validators.required],
    });
    this.cardTypeCbx.filterChange
    .asObservable()
    .pipe(
      debounceTime(300),
      tap(() => (this.cardTypeCbx.loading = true)),
      switchMap((value) => this.searchCardTypes(value)
      )
    )
    .subscribe((x) => {
      this.cardTypeSimpleFilter = x.items;
      this.cardTypeCbx.loading = false;
    });
    this.loadCardTypes();
    this.getDefault();
  }

  onSave(){
    this.submitted = true;
    this.touched();
    if (this.formGroup.invalid)
      return;
    var val = this.formGroup.value;
    val.typeId = val.type ? val.type.id : '';
    val.partnerId = this.partnerId || '';
    this.cardCardsService.create(val).subscribe(res => {
      this.cardCardsService.buttonActive([res.id]).subscribe(()=> {
        this.notifyService.notify('success','Lưu và kích hoạt thành công');
        this.activeModal.close();
      })
    })
  }

  getDefault(){
    this.cardCardsService.getDefault().subscribe(res => {
      this.formGroup.get("type").setValue(res.type);
    })
  }

  loadCardTypes(){
    this.searchCardTypes().subscribe(result => {
      this.cardTypeSimpleFilter = result.items;
    })
  }

  searchCardTypes(q?: string) {
    var val = {search: q || '', offset: 0, limit: 0};
    return this.cardService.getPaged(val);
  }

  touched() {
    (<any>Object).values(this.formGroup.controls).forEach((control: FormControl) => {
      control.markAsTouched();
    })
    return;
  }

  get f(){
    return this.formGroup.controls;
  }


}

export function createLengthValidator(): ValidatorFn {
  return (control:AbstractControl) : ValidationErrors | null => {
      const valueLength = control.value.toString().length;
      var lengthValid = true;
      if (valueLength < 10 || valueLength > 15)
        lengthValid = false;
      return !lengthValid ? {lengthError:true}: null;
  }
}
