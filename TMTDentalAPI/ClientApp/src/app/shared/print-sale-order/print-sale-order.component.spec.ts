import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintSaleOrderComponent } from './print-sale-order.component';

describe('PrintSaleOrderComponent', () => {
  let component: PrintSaleOrderComponent;
  let fixture: ComponentFixture<PrintSaleOrderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintSaleOrderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintSaleOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
