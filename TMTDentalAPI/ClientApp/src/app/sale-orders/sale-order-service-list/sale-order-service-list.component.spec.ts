import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderServiceListComponent } from './sale-order-service-list.component';

describe('SaleOrderServiceListComponent', () => {
  let component: SaleOrderServiceListComponent;
  let fixture: ComponentFixture<SaleOrderServiceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderServiceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderServiceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
