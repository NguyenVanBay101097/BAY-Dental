import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockPickingIncomingListComponent } from './stock-picking-incoming-list.component';

describe('StockPickingIncomingListComponent', () => {
  let component: StockPickingIncomingListComponent;
  let fixture: ComponentFixture<StockPickingIncomingListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockPickingIncomingListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockPickingIncomingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
