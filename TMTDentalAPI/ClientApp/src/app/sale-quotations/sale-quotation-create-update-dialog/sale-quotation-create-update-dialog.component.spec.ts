import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleQuotationCreateUpdateDialogComponent } from './sale-quotation-create-update-dialog.component';

describe('SaleQuotationCreateUpdateDialogComponent', () => {
  let component: SaleQuotationCreateUpdateDialogComponent;
  let fixture: ComponentFixture<SaleQuotationCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleQuotationCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleQuotationCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
