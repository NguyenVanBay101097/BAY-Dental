import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockPickingListComponent } from './stock-picking-list.component';

describe('StockPickingListComponent', () => {
  let component: StockPickingListComponent;
  let fixture: ComponentFixture<StockPickingListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockPickingListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockPickingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
