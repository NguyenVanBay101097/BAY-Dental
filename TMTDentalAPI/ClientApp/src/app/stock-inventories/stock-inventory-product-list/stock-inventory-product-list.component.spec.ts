import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockInventoryProductListComponent } from './stock-inventory-product-list.component';

describe('StockInventoryProductListComponent', () => {
  let component: StockInventoryProductListComponent;
  let fixture: ComponentFixture<StockInventoryProductListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockInventoryProductListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockInventoryProductListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
