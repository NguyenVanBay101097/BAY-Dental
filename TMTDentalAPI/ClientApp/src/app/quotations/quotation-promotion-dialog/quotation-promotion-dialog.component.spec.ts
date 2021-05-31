import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QuotationPromotionDialogComponent } from './quotation-promotion-dialog.component';

describe('QuotationPromotionDialogComponent', () => {
  let component: QuotationPromotionDialogComponent;
  let fixture: ComponentFixture<QuotationPromotionDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QuotationPromotionDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QuotationPromotionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
