import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleQuotationListComponent } from './sale-quotation-list.component';

describe('SaleQuotationListComponent', () => {
  let component: SaleQuotationListComponent;
  let fixture: ComponentFixture<SaleQuotationListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleQuotationListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleQuotationListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
