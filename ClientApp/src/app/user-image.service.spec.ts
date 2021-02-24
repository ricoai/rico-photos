import { TestBed } from '@angular/core/testing';

import { UserImageService } from './user-image.service';

describe('UserImageService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UserImageService = TestBed.get(UserImageService);
    expect(service).toBeTruthy();
  });
});
