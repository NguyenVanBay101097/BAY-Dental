import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderDotkhamListComponent } from './sale-order-dotkham-list.component';

describe('SaleOrderDotkhamListComponent', () => {
  let component: SaleOrderDotkhamListComponent;
  let fixture: ComponentFixture<SaleOrderDotkhamListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderDotkhamListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderDotkhamListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
