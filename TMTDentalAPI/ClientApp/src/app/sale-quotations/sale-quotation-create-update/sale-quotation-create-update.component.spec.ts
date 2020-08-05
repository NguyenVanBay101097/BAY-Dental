import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleQuotationCreateUpdateComponent } from './sale-quotation-create-update.component';

describe('SaleQuotationCreateUpdateComponent', () => {
  let component: SaleQuotationCreateUpdateComponent;
  let fixture: ComponentFixture<SaleQuotationCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleQuotationCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleQuotationCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
