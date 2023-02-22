import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockPickingOutgoingListComponent } from './stock-picking-outgoing-list.component';

describe('StockPickingOutgoingListComponent', () => {
  let component: StockPickingOutgoingListComponent;
  let fixture: ComponentFixture<StockPickingOutgoingListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockPickingOutgoingListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockPickingOutgoingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
