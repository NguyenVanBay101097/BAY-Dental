import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderCuFormComponent } from './sale-order-cu-form.component';

describe('SaleOrderCuFormComponent', () => {
  let component: SaleOrderCuFormComponent;
  let fixture: ComponentFixture<SaleOrderCuFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderCuFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderCuFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
