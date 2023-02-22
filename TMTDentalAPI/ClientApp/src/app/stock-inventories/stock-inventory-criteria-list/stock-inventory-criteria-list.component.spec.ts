import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockInventoryCriteriaListComponent } from './stock-inventory-criteria-list.component';

describe('StockInventoryCriteriaListComponent', () => {
  let component: StockInventoryCriteriaListComponent;
  let fixture: ComponentFixture<StockInventoryCriteriaListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockInventoryCriteriaListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockInventoryCriteriaListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
