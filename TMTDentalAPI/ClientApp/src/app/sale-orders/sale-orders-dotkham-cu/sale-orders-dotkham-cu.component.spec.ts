import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrdersDotkhamCuComponent } from './sale-orders-dotkham-cu.component';

describe('SaleOrdersDotkhamCuComponent', () => {
  let component: SaleOrdersDotkhamCuComponent;
  let fixture: ComponentFixture<SaleOrdersDotkhamCuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrdersDotkhamCuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrdersDotkhamCuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
