import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedSaleOrderGridComponent } from './shared-sale-order-grid.component';

describe('SharedSaleOrderGridComponent', () => {
  let component: SharedSaleOrderGridComponent;
  let fixture: ComponentFixture<SharedSaleOrderGridComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SharedSaleOrderGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SharedSaleOrderGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
