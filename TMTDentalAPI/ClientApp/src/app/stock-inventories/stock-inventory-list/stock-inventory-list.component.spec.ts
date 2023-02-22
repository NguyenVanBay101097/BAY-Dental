import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockInventoryListComponent } from './stock-inventory-list.component';

describe('StockInventoryListComponent', () => {
  let component: StockInventoryListComponent;
  let fixture: ComponentFixture<StockInventoryListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockInventoryListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockInventoryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
